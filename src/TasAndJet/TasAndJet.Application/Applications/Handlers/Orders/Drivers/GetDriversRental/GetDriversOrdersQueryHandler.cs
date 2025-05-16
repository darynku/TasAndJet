using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class GetDriversOrdersQueryHandler(
    ApplicationDbContext context, 
    IFileProvider fileProvider) : IRequestHandler<GetDriversOrdersQuery, PagedList<DriverOrderResponse>> 
{
    /// <summary>
    /// Метод для получения заказов со статусом Created
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Лист заказов вместе с пагинацией</returns>
    public async Task<PagedList<DriverOrderResponse>> Handle(GetDriversOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = context.Orders
            .Include(o => o.Driver)
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Created && o.IsDriverOrder == true); // Фильтр по статусу "Создан" для водителей

        // Поиск по описанию
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search?.ToLower();
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

        if (request.City.HasValue)
        {
            orderQuery = orderQuery.Where(o => o.City == request.City.Value);
        }
        
        var orderPagedList = await orderQuery
            .OrderByDescending(o => o.OrderDate) // Сортировка по дате создания
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return ConvertToResponse(orderPagedList);
    }

    private PagedList<DriverOrderResponse> ConvertToResponse(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items
            .Select(order => new DriverOrderResponse()
            {
                OrderId = order.Id,
                ClientId = order.ClientId,
                DriverId = order.DriverId,
                PhoneNumber = order.Driver?.PhoneNumber ?? string.Empty,
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
                ImageUrls = (order.ImageKeys ?? [])
                    .Select(fileProvider.GeneratePreSignedUrl).ToList() 
            }).ToList();

        return new PagedList<DriverOrderResponse>()
        {
            Items = orders,
            Page = orderPagedList.Page,
            PageSize = orderPagedList.PageSize,
            TotalCount = orderPagedList.TotalCount
        };
    }

}
