using Microsoft.AspNetCore.Http;
using TasAndJet.Domain.Entities.Enums;
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
    public required KazakhstanCity City { get; set; }
    public required OrderType OrderType { get; set; }
    
    // Rental
    public DateTime? RentalStartDate { get; }
    public DateTime? RentalEndDate { get; }
    public IFormFileCollection? Images { get; }
    // Freight
    public decimal? CargoWeight { get; }
    public string? CargoType { get; }
}