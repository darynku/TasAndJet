using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class GerOrdersQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrdersQuery, PagedList<OrderResponse>>
{
    public async Task<PagedList<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = context.Orders;
        var orderPagedList = await orderQuery.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
        return ConvertToResponseAsync(orderPagedList);
    }

    private PagedList<OrderResponse> ConvertToResponseAsync(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items
            .Select(orderDto => new OrderResponse
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
            })
            .ToList();

        return new PagedList<OrderResponse>()
        {
            Items = orders,
            Page = orderPagedList.Page,
            PageSize = orderPagedList.PageSize,
            TotalCount = orderPagedList.TotalCount
        };

    }
}