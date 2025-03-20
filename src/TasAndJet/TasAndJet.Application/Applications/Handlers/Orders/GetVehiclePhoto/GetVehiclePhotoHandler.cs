using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.GetVehiclePhoto;

public record GetVehiclePhotoCommand(Guid Id) : IRequest<string>;

public class GetVehiclePhotoCommandHandler(IFileProvider fileProvider, ApplicationDbContext context)
    : IRequestHandler<GetVehiclePhotoCommand, string>
{
    public async Task<string> Handle(GetVehiclePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Vehicles
                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        var fileKey = user.PhotoUrl;

        if (string.IsNullOrEmpty(fileKey))
            throw new NotFoundException("У пользователя не загружен аватар");

        var preSignedUrl = fileProvider.GeneratePreSignedUrl(fileKey);

        return preSignedUrl;
    }
}