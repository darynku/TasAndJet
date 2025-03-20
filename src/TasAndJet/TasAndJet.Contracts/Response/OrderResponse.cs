using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Response;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public Guid ClientId { get; set; }
    public required string Description { get; set; } 
    public required string PickupAddress { get; set; }
    public required string DestinationAddress  { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}