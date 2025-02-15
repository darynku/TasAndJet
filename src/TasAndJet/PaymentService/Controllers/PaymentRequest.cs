namespace PaymentService.Controllers;

public class PaymentRequest
{
    public required string CustomerId { get; set; } 
    public long Amount { get; set; }
}