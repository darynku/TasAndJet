using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasAndJet.Api.Common;
using TasAndJet.Api.Common.Paged;
using TasAndJet.Api.Contracts.Response;
using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Infrastructure;

namespace TasAndJet.Api.Applications.Handlers.Orders.Get;

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
                Description = orderDto.Description,
                PickupAddress = orderDto.PickupAddress,
                DestinationAddress = orderDto.DestinationAddress,
                OrderDate = orderDto.OrderDate,
                Status = orderDto.Status,
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