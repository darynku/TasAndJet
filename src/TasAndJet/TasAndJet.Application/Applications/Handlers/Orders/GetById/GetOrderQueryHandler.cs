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
        var result = await context.Orders.Select(order => new OrderResponse
        {
            OrderId = order.Id,
            ClientId = order.ClientId,
            Description = order.Description,
            PickupAddress = order.PickupAddress,
            DestinationAddress = order.DestinationAddress,
            OrderDate = order.OrderDate,
            Status = order.Status,
            VehicleType = order.VehicleType,
            Region = order.Region,
            TotalPrice = order.TotalPrice,
            
        }).FirstOrDefaultAsync(orderDto => orderDto.OrderId == request.OrderId, cancellationToken);
        if (result == null)
            return Errors.General.NotFound(result?.OrderId, "Заказ не найден");

        return Result.Success<OrderResponse, Error>(result);
    }
}