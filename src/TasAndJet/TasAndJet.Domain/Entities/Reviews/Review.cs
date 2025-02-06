namespace TasAndJet.Domain.Entities.Reviews;
public class Review
{
    private Review()
    {
    }

    private Review(Guid id, Guid clientId, Guid driverId, Guid orderId, string comment, int rating)
    {
        Id = id;
        ClientId = clientId;
        DriverId = driverId;
        OrderId = orderId;
        Comment = comment;
        Rating = rating;
    }

    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid DriverId { get; set; }
    public Guid OrderId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    
    public static Review Create(Guid id, Guid clientId, Guid driverId, Guid orderId, string comment, int rating)
    {
        return new Review(id, clientId, driverId, orderId, comment, rating);
    }

}
