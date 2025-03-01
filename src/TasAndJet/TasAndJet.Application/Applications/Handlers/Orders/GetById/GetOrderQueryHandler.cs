using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.GetById;

public class GetOrderQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrderQuery, Result<OrderResponse, Error>>
{
    public async Task<Result<OrderResponse, Error>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Orders.Select(orderDto => new OrderResponse
        {
            OrderId = orderDto.Id,
            ClientId = orderDto.ClientId,
            DriverId = orderDto.DriverId,
            Description = orderDto.Description,
            PickupAddress = orderDto.PickupAddress,
            DestinationAddress = orderDto.DestinationAddress,
            OrderDate = orderDto.OrderDate,
            Status = orderDto.Status,
            Service = new ServiceResponse()
            {
                Title = orderDto.Service.Title,
                VehicleType = orderDto.Service.Vehicle.VehicleType,
                PhotoUrl = orderDto.Service.Vehicle.PhotoUrl,
                ServiceType = orderDto.Service.ServiceType
            }
        }).FirstOrDefaultAsync(orderDto => orderDto.OrderId == request.OrderId, cancellationToken);
        if (result == null)
            return Errors.General.NotFound(result?.OrderId, "Заказ не найден");

        return Result.Success<OrderResponse, Error>(result);
    }
}