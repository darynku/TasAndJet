using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Api.Requests;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Options;

namespace TasAndJet.Api.Controllers;


[SwaggerTag("Контроллер для работы с месячной подпиской для водителя")]
public class SubscriptionController : ApplicationController
{
    private readonly StripeOptions _stripeOptions;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SubscriptionController> _logger;
    public SubscriptionController(IOptions<StripeOptions> stripeOptions, ApplicationDbContext dbContext, ILogger<SubscriptionController> logger)
    {
        _stripeOptions = stripeOptions.Value;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession(CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _dbContext.Users
                .Include(user => user.UserSubscription)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
            if (user is null)
                return NotFound("Пользователь не найден");

            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}",
                    Phone = user.PhoneNumber
                }, cancellationToken: cancellationToken);

                user.SetStripeCustomerId(customer.Id);
        
                _logger.LogInformation("Stripe customer created {CustomerId}", customer.Id);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                Customer = user.StripeCustomerId,
                Mode = "subscription",
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Price = _stripeOptions.PriceId,
                        Quantity = 1,
                    }
                ],
                SuccessUrl = _stripeOptions.SuccessUrl,
                CancelUrl = _stripeOptions.CancelUrl,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Transaction commited");
            
            return Ok(new CheckoutSessionResponse(session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error while creating checkout session {Message}", ex.Message);
            
            await transaction.RollbackAsync(cancellationToken);
            
            return BadRequest("Ошибка при выполнении подписки");
        }
    }

    /// <summary>
    /// Обработка вебхуков от Stripe.
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
        _logger.LogInformation("Json created");

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeOptions.WebhookSecret
            );

            _logger.LogInformation("Stripe event created: {Event}", stripeEvent.Type);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Webhook error: {Message}", ex.Message);
            return BadRequest("Webhook error");
        }

        switch (stripeEvent.Type)
        {
            case "invoice.payment_succeeded":
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice is null)
                    break;

                var user = await _dbContext.Users.FirstOrDefaultAsync(
                    u => u.StripeCustomerId == invoice.CustomerId,
                    cancellationToken
                );
                if (user is null)
                    break;

                var existingSubscription = await _dbContext.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.StripeSubscriptionId == invoice.SubscriptionId, cancellationToken);

                if (existingSubscription == null)
                {
                    var subscriptionStart = invoice.Lines.Data[0].Period.Start;
                    var subscriptionEnd = invoice.Lines.Data[0].Period.End;

                    var newSubscription = UserSubscription.Create(
                        Guid.NewGuid(),
                        user.Id,
                        invoice.SubscriptionId,
                        subscriptionStart,
                        subscriptionEnd
                    );

                    _dbContext.UserSubscriptions.Add(newSubscription);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Создана новая подписка для пользователя {UserId}", user.Id);
                }

                break;
            }

            case "invoice.payment_failed":
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice is null)
                    break;

                var sub = await _dbContext.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.StripeSubscriptionId == invoice.SubscriptionId, cancellationToken);

                if (sub != null)
                {
                    var subscriptionStart = invoice.Lines.Data[0].Period.Start;
                    sub.SetEndDate(subscriptionStart);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Подписка {SubId} завершена по причине неудачного платежа", sub.Id);
                }

                break;
            }

            case "customer.subscription.updated":
            case "customer.subscription.created":
                // Эти события можешь обработать при необходимости
                _logger.LogInformation("Subscription event (created/updated) received");
                break;

            default:
                _logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                break;
        }

        return Ok();
    }


    [HttpGet("check/{userId}")]
    public async Task<IActionResult> CheckSubscription([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserSubscription)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null) 
            return NotFound("Пользователь не найден");

        var isActive = user.HasActiveSubscription();
        return Ok(isActive);
    }

    [HttpPost("cancel/{userId}")]
    public async Task<IActionResult> CancelSubscription(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.Include(u => u.UserSubscription)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null) return NotFound("Пользователь не найден");

        var activeSubscription = user.UserSubscription;

        var subscriptionService = new SubscriptionService();
        await subscriptionService.CancelAsync(activeSubscription.StripeSubscriptionId,
            cancellationToken: cancellationToken);

        activeSubscription.SetEndDate(DateTime.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok("Подписка отменена");
    }

    [HttpGet("success")]
    public IActionResult Success()
    {
        return Ok("Success");
    }

    [HttpGet("cancel")]
    public IActionResult Cancel()
    {
        return Ok("Cancel");
    }

}