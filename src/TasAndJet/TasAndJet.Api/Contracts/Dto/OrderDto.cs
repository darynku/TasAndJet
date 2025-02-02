using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Entities.Services;

namespace TasAndJet.Api.Contracts.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    public required string ClientName { get; set; }
    public required string DriverName { get; set; }
    public string Description { get; set; }
    public string PickupAddress { get; set; }
    public string DestinationAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public Service Service { get; set; }
}