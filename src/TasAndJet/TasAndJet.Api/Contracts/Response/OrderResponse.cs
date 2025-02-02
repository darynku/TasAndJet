using TasAndJet.Api.Entities.Orders;

namespace TasAndJet.Api.Contracts.Response;

public class OrderResponse
{
    public string Description { get; set; } 
    public string PickupAddress { get; set; }
    public string DestinationAddress  { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public OrderStatus Status { get; set; }
}