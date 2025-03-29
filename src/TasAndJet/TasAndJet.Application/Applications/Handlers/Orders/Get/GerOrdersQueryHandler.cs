using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class GetOrdersQueryHandler(ApplicationDbContext context) 
    : IRequestHandler<GetOrdersQuery, PagedList<OrderResponse>> 
{
    /// <summary>
    /// Метод для получения заказов со статусом Created
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Лист заказов вместе с пагинацией</returns>
    public async Task<PagedList<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = context.Orders
            .Where(o => o.Status == OrderStatus.Created); // Фильтр по статусу "Создан"

        // Поиск по описанию
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            orderQuery = orderQuery.Where(o => o.Description.ToLower().Contains(searchLower));
        }

        // Фильтр по типу транспорта
        if (request.VehicleTypeSearch.HasValue)
        {
            orderQuery = orderQuery.Where(o => o.VehicleType == request.VehicleTypeSearch);
        }

        // Фильтр по цене
        if (request.MinPrice.HasValue)
        {
            orderQuery = orderQuery.Where(o => o.TotalPrice >= request.MinPrice);
        }
        if (request.MaxPrice.HasValue)
        {
            orderQuery = orderQuery.Where(o => o.TotalPrice <= request.MaxPrice);
        }

        // Фильтр по региону
        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            orderQuery = orderQuery.Where(o => o.Region == request.Region);
        }

        var orderPagedList = await orderQuery
            .OrderByDescending(o => o.OrderDate) // Сортировка по дате создания
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return ConvertToResponse(orderPagedList);
    }

    private static PagedList<OrderResponse> ConvertToResponse(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items
            .Select(order => new OrderResponse
            {
                OrderId = order.Id,
                ClientId = order.ClientId,
                Description = order.Description,
                PickupAddress = order.PickupAddress,
                DestinationAddress = order.DestinationAddress,
                OrderDate = order.OrderDate,
                Status = order.Status,
                VehicleType= order.VehicleType,
                TotalPrice = order.TotalPrice,
                Region = order.Region
            }).ToList();

        return new PagedList<OrderResponse>()
        {
            Items = orders,
            Page = orderPagedList.Page,
            PageSize = orderPagedList.PageSize,
            TotalCount = orderPagedList.TotalCount
        };
    }
}
