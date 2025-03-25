using MediatR;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;

namespace TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;

public class UploadUserPhotoCommandHandler(IUploadFileService service) : IRequestHandler<UploadUserPhotoCommand>
{
    public async Task Handle(UploadUserPhotoCommand request, CancellationToken cancellationToken)
    {
        await service.HandleUserAvatar(request.UserId, request.File, cancellationToken);
    }
}