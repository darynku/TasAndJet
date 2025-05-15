using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateDriverRental;

public sealed class CreateDriverRentalCommand : IRequest
{
    public Guid DriverId { get; init; }
    public required string Description { get; init; }
    public DateTime RentalStartDate { get; init; }
    public DateTime RentalEndDate { get; init; }
    public decimal TotalPrice { get; init; }
    public VehicleType VehicleType { get; init; }
    public KazakhstanCity City { get; init; }
    public IFormFileCollection? Images { get; init; }
}

public class CreateDriverRentalCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateDriverRentalCommandHandler> logger,
    IUploadFileService fileService) : IRequestHandler<CreateDriverRentalCommand>
{
    public async Task Handle(CreateDriverRentalCommand request, CancellationToken cancellationToken)
    {
        var driver = await context.Users.FirstOrDefaultAsync(u => u.Id == request.DriverId, cancellationToken)
                     ?? throw new NotFoundException("Такого пользователя не существует");

        var order = driver.CreateDriverRentalOrder(
            Guid.NewGuid(),
            request.Description,
            request.RentalStartDate,
            request.RentalEndDate,
            request.TotalPrice,
            request.VehicleType,
            request.City
        );

        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        if (request.Images is { Count: > 0 })
        {
            await fileService.HandleOrderPhotos(order.Id, request.Images, cancellationToken);
        }

        logger.LogInformation("Арендный заказ создан {Id}, от водителя {ClientId}", order.Id, driver.Id);

        driver.AddDriverOrder(order);
        await context.SaveChangesAsync(cancellationToken);

    }
}
