namespace PaymentService.Controllers;

public class AddCardRequest
{
    public required string CustomerId { get; set; } 
    public required string CardNumber { get; set; } 
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public required string Cvc { get; set; } 
}