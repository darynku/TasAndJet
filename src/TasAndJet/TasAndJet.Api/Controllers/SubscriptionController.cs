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
        
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            if (user.UserSubscription.IsPremium())
            {
                return BadRequest("У пользователя уже есть активная подписка.");
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
            Session session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return Ok(new CheckoutSessionResponse(session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error while creating checkout session {Message}", ex.Message);
            
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
        Event stripeEvent;
        
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeOptions.WebhookSecret
            );
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Webhook error: {Message}", ex.Message);
            return BadRequest( "Webhook error");
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.InvoicePaymentSucceeded:
                
                var firstInvoice = stripeEvent.Data.Object as Invoice;
                
                if(firstInvoice is null) break;
                
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == firstInvoice.CustomerId, cancellationToken);
                
                if (user == null) break;

                var existingSubscription = await _dbContext.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.StripeSubscriptionId == firstInvoice.SubscriptionId, cancellationToken);

                if (existingSubscription == null)
                {
                    var subscriptionStart = firstInvoice.Lines.Data[0].Period.Start;
                    var subscriptionEnd = firstInvoice.Lines.Data[0].Period.End;
                    
                    var newSubscription = UserSubscription.Create(
                        Guid.NewGuid(),
                        user.Id,
                        firstInvoice.SubscriptionId, 
                        subscriptionStart,
                        subscriptionEnd);
                    
                    await _dbContext.UserSubscriptions.AddAsync(newSubscription);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                break;
            
            case EventTypes.InvoicePaymentFailed:
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice?.Customer == null) break;

                var sub = await _dbContext.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.StripeSubscriptionId == invoice.SubscriptionId, cancellationToken);

                if (sub != null)
                {
                    var subscriptionStart = invoice.Lines.Data[0].Period.Start;
                    sub.SetEndDate(subscriptionStart);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
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