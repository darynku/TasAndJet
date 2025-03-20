using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class GerOrdersQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrdersQuery, PagedList<OrderResponse>>
{
    /// <summary>
    /// Метод для получения заказов со статусом Created
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Лист заказов вместе с пагинацией</returns>
    public async Task<PagedList<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = context.Orders;
        var orderPagedList = await orderQuery.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
        return ConvertToResponseAsync(orderPagedList);
    }

    private PagedList<OrderResponse> ConvertToResponseAsync(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items
            .Where(orderDto => orderDto.Status == OrderStatus.Created)
            .Select(orderDto => new OrderResponse
            {
                OrderId = orderDto.Id,
                ClientId = orderDto.ClientId,
                Description = orderDto.Description,
                PickupAddress = orderDto.PickupAddress,
                DestinationAddress = orderDto.DestinationAddress,
                OrderDate = orderDto.OrderDate,
                Status = orderDto.Status
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