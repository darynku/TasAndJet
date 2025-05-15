using MediatR;
using Microsoft.AspNetCore.Http;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateRental;

public class CreateRentalOrderCommand : IRequest<Guid>
{
    public Guid ClientId { get; init; }
    public string Description { get; init; }
    public DateTime RentalStartDate { get; init; }
    public DateTime RentalEndDate { get; init; }
    public decimal TotalPrice { get; init; }
    public VehicleType VehicleType { get; init; }
    public KazakhstanCity City { get; init; }
    public IFormFileCollection? Images { get; init; }
}
