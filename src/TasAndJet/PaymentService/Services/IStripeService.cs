using Stripe;

namespace PaymentService.Services;

public interface IStripeService
{
    Task<Customer> CreateCustomerAsync(string email, string name);
    Task<PaymentMethod> AddCardToCustomerAsync(string customerId, string cardNumber, int expMonth, int expYear, string cvc);
    Task<PaymentIntent> PayWithSavedCardAsync(string customerId, long amount);
    Task<Transfer> TransferFundsAsync(long amount, string destinationAccount);
}
