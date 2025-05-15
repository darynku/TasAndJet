using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Handlers.Orders.GetById;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.GetById;

public record GetByIdDriverOrderQuery(Guid OrderId) : IRequest<Result<DriverOrderDetailsResponse, Error>>;

public class DriverOrderDetailsResponse
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public Guid? DriverId { get; set; }
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

    // Rental
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public List<string>? ImageUrls { get; set; }
}
