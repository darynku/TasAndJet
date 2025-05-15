using MediatR;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;

namespace TasAndJet.Application.Applications.Handlers.Orders.UploadFile;

public class UploadVehiclePhotoCommandHandler(IUploadFileService fileService) : IRequestHandler<UploadVehiclePhotoCommand>
{
    public async Task Handle(UploadVehiclePhotoCommand request, CancellationToken cancellationToken)
    {
        await fileService.HandleVehiclePhoto(request.VehicleId, request.File, cancellationToken);
    }
}