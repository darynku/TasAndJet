using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class ClientQueryHandler(ApplicationDbContext context, IFileProvider fileProvider)
: IRequestHandler<ClientQuery, PagedList<OrderResponse>>
{
    /// <summary>
    /// Получение заказов по ClientId
    /// </summary>
    public async Task<PagedList<OrderResponse>> Handle(ClientQuery request, CancellationToken cancellationToken)
    {
        var query = context.Orders
        .Where(o => o.ClientId == request.clientId);

        var pagedOrders = await query
        .OrderByDescending(o => o.OrderDate)
        .ToPagedListAsync(page: 1, pageSize: 10, cancellationToken); // Можно заменить на параметры, если появятся

        return ConvertToResponse(pagedOrders);
    }

    private PagedList<OrderResponse> ConvertToResponse(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items.Select(order =>
        {
            var response = new OrderResponse
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
                OrderType = order.OrderType
            };

            switch (order.OrderType)
            {
                case OrderType.Rental:
                    response.RentalStartDate = order.RentalStartDate;
                    response.RentalEndDate = order.RentalEndDate;
                    response.ImageUrls = (order.ImageKeys ?? [])
                    .Select(fileProvider.GeneratePreSignedUrl)
                    .ToList();
                    break;

                case OrderType.Freight:
                    response.CargoWeight = order.CargoWeight;
                    response.CargoType = order.CargoType;
                    break;

                default:
                    throw new Exception("Неизвестный тип заказа");
            }

            return response;
        }).ToList();

        return new PagedList<OrderResponse>
        {
            Items = orders,
            Page = orderPagedList.Page,
            PageSize = orderPagedList.PageSize,
            TotalCount = orderPagedList.TotalCount
        };
    }
}
