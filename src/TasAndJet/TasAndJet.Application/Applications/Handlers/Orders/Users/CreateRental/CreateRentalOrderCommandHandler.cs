using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateRental;

public class CreateRentalOrderCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateRentalOrderCommandHandler> logger,
    IUploadFileService fileService,
    IValidator<CreateRentalOrderCommand> validator) : IRequestHandler<CreateRentalOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalOrderCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAsync(request, cancellationToken);
        
        var client = await context.Users
                         .FirstOrDefaultAsync(u => u.Id == request.ClientId, cancellationToken)
                     ?? throw new NotFoundException("Такого пользователя не существует");

        var order = client.CreateRentalOrder(
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

        logger.LogInformation("Арендный заказ создан {Id}", order.Id);

        client.AddClientOrder(order);
        await context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
