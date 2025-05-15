using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Response;

public class DriverOrderResponse
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public Guid? DriverId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Description { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public VehicleType VehicleType { get; set; }
    public required string Region { get; set; }
    public decimal TotalPrice { get; set; }
    public KazakhstanCity City { get; set; }

    // Только для аренды
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public List<string> ImageUrls { get; set; } = [];
}