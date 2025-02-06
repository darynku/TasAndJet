namespace TasAndJet.Contracts.Data.Review;

public class ReviewData
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid DriverId { get; set; }
    public Guid OrderId { get; set; }
    public required string Comment { get; set; }
    public required int Rating { get; set; }
}