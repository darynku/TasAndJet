using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Vehicles.GetVehiclePhoto;

public record GetVehiclePhotoCommand(Guid Id) : IRequest<string>;