using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Data.Orders;

public class OrderData
{
    public Guid ClientId { get; set; }
    public required string Description { get; set; } 
    public required string PickupAddress { get; set; }
    public required string DestinationAddress  { get; set; }
    
    public required DateTime OrderDate { get; set; }
    public required decimal TotalPrice { get; set; }
    public required VehicleType VehicleType { get; set; }
}