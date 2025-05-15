using MediatR;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateFreight;

public class CreateFreightOrderCommand : IRequest<Guid>
{
    public Guid ClientId { get; init; }
    public string Description { get; init; }
    public string PickupAddress { get; init; }
    public string DestinationAddress { get; init; }
    public decimal CargoWeight { get; init; }
    public string CargoType { get; init; }
    public decimal TotalPrice { get; init; }
    public VehicleType VehicleType { get; init; }
    public KazakhstanCity City { get; init; }
}