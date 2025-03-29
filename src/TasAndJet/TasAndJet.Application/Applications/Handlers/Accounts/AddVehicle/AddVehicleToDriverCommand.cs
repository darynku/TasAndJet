using MediatR;
using Microsoft.AspNetCore.Http;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Accounts.AddVehicle;

public class AddVehicleToDriverCommand : IRequest
{
    public Guid UserId { get; set; }
    public required string Mark { get; set; }
    public required VehicleType VehicleType { get; set; }
    public required double Capacity { get; set; }
    public required IFormFile? PhotoUrl { get; set; }
}