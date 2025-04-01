using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Response;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public string Description { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public VehicleType VehicleType { get; set; }
    public string Region { get; set; }
    public decimal TotalPrice { get; set; }
    public KazakhstanCity City { get; set; }

    // Только для аренды
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }

    // Только для грузоперевозки
    public decimal? CargoWeight { get; set; }
    public string? CargoType { get; set; }

    // Картинки для аренды
    public List<string> ImageUrls { get; set; } = [];
}
