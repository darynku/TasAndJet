using Stripe;

namespace PaymentService.Services;

public class StripeService : IStripeService
{
    public async Task<Customer> CreateCustomerAsync(string email, string name)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };
        var service = new CustomerService();
        return await service.CreateAsync(options);
    }

    public async Task<PaymentMethod> AddCardToCustomerAsync(string customerId, string cardNumber, int expMonth, int expYear, string cvc)
    {
        var tokenService = new TokenService();
        var cardToken = await tokenService.CreateAsync(new TokenCreateOptions
        {
            Card = new TokenCardOptions
            {
                Number = cardNumber,
                ExpMonth = expMonth.ToString(),
                ExpYear = expYear.ToString(),
                Cvc = cvc
            }
        });

        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions
            {
                Token = cardToken.Id
            }
        });

        // Привязка метода оплаты к клиенту
        await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
        {
            Customer = customerId
        });

        return paymentMethod;
    }

    public async Task<PaymentIntent> PayWithSavedCardAsync(string customerId, long amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "kzt",
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card" }
        };
        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }

    public async Task<Transfer> TransferFundsAsync(long amount, string destinationAccount)
    {
        var options = new TransferCreateOptions
        {
            Amount = amount,
            Currency = "kzt",
            Destination = destinationAccount,
            TransferGroup = "ORDER_123"
        };
        var service = new TransferService();
        return await service.CreateAsync(options);
    }
}
