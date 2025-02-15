using Microsoft.AspNetCore.Mvc;
using PaymentService.Services;

namespace PaymentService.Controllers;
[ApiController]
[Route("api/payments")]
public class StripeController(IStripeService stripeService) : ControllerBase
{
    [HttpPost("create-customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerRequest request)
    {
        var customer = await stripeService.CreateCustomerAsync(request.Email, request.Name);
        return Ok(new { CustomerId = customer.Id });
    }

    [HttpPost("add-card")]
    public async Task<IActionResult> AddCard([FromBody] AddCardRequest request)
    {
        var paymentMethod = await stripeService.AddCardToCustomerAsync(request.CustomerId, request.CardNumber, request.ExpMonth, request.ExpYear, request.Cvc);
        return Ok(new { PaymentMethodId = paymentMethod.Id });
    }


    [HttpPost("pay")]
    public async Task<IActionResult> PayWithSavedCard([FromBody] PaymentRequest request)
    {
        var paymentIntent = await stripeService.PayWithSavedCardAsync(request.CustomerId, request.Amount);
        return Ok(new { ClientSecret = paymentIntent.ClientSecret });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferFunds([FromBody] TransferRequest request)
    {
        var transfer = await stripeService.TransferFundsAsync(request.Amount, request.DestinationAccount);
        return Ok(new { TransferId = transfer.Id });
    }
}