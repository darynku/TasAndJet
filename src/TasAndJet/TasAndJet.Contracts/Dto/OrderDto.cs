using TasAndJet.Domain.Entities.Orders;

namespace TasAndJet.Contracts.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    public required string ClientName { get; set; }
    public string? DriverName { get; set; }
    public string Description { get; set; }
    public string PickupAddress { get; set; }
    public string DestinationAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}