using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Accounts.AddVehicle;

public class AddVehicleToDriverCommandHandler(ApplicationDbContext context, IUploadFileService uploadFileService)
    : IRequestHandler<AddVehicleToDriverCommand>
{
    public async Task Handle(AddVehicleToDriverCommand request,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        if (user.Role.Name != Role.Driver)
        {
            throw new RoleDeniedAccessException("Запрос на обработку отклонен, неверная роль");
        }

        var vehiclePhoto = string.Empty;

        var vehicle = Vehicle.Create(
            Guid.NewGuid(),
            request.UserId,
            request.VehicleType,
            request.Mark,
            request.Number,
            request.Colour,
            request.Capacity,
            vehiclePhoto);

        await context.Vehicles.AddAsync(vehicle, cancellationToken);
        user.AddVehicle(vehicle);
        await context.SaveChangesAsync(cancellationToken);

        if (request.PhotoUrl is not null)
            await uploadFileService.HandleVehiclePhoto(user.Id, request.PhotoUrl, cancellationToken);
    }
}