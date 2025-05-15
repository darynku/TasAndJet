using MediatR;
using Microsoft.AspNetCore.Http;

namespace TasAndJet.Application.Applications.Handlers.Orders.UploadFile;

public record UploadVehiclePhotoCommand(Guid VehicleId, IFormFile File) : IRequest;