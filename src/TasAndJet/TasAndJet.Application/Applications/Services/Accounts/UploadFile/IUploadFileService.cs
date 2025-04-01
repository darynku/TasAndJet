using Microsoft.AspNetCore.Http;
using TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;

namespace TasAndJet.Application.Applications.Services.Accounts.UploadFile;

public interface IUploadFileService
{
    Task HandleUserAvatar(Guid userId, IFormFile fileRequest, CancellationToken cancellationToken);
    Task HandleVehiclePhoto(Guid vehicleId, IFormFile fileRequest, CancellationToken cancellationToken);
    Task HandleOrderPhotos(Guid orderId, IFormFileCollection files, CancellationToken cancellationToken);
}