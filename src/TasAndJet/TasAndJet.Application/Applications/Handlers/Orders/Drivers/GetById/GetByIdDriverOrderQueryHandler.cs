using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.GetById;

public class GetByIdDriverOrderQueryHandler(ApplicationDbContext context, IFileProvider fileProvider)
    : IRequestHandler<GetByIdDriverOrderQuery, Result<DriverOrderDetailsResponse, Error>>
{
    public async Task<Result<DriverOrderDetailsResponse, Error>> Handle(GetByIdDriverOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            return Errors.General.NotFound(request.OrderId, "Заказ не найден");

        var response = new DriverOrderDetailsResponse
        {
            OrderId = order.Id,
            ClientId = order.ClientId,
            DriverId = order.DriverId,
            Description = order.Description,
            PickupAddress = order.PickupAddress,
            DestinationAddress = order.DestinationAddress,
            OrderDate = order.OrderDate,
            Status = order.Status,
            VehicleType = order.VehicleType,
            Region = order.Region,
            TotalPrice = order.TotalPrice,
            City = order.City,
            OrderType = order.OrderType,
            RentalStartDate = order.RentalStartDate,
            RentalEndDate = order.RentalEndDate,
            ImageUrls = order.ImageKeys.Select(fileProvider.GeneratePreSignedUrl).ToList()
        };
        return Result.Success<DriverOrderDetailsResponse, Error>(response);
    }
}