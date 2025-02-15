namespace PaymentService.Controllers;

public class TransferRequest
{
    public long Amount { get; set; }
    public required string DestinationAccount { get; set; } 
}