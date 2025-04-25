using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public class GetOrdersQueryHandler(ApplicationDbContext context, IFileProvider fileProvider) 
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

        if (request.City.HasValue)
        {
            orderQuery = orderQuery.Where(o => o.City == request.City.Value);
        }
        
        var orderPagedList = await orderQuery
            .OrderByDescending(o => o.OrderDate) // Сортировка по дате создания
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return ConvertToResponse(orderPagedList);
    }

    private PagedList<OrderResponse> ConvertToResponse(PagedList<Order> orderPagedList)
    {
        var orders = orderPagedList.Items
            .Select(order =>
            {
                var response = new OrderResponse
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
                    // Только для грузоперевозки
                    case OrderType.Freight:
                        response.CargoWeight = order.CargoWeight;
                        response.CargoType = order.CargoType;
                        break;
                  
                    default:
                        throw new Exception("Неизвестный тип заказа");
                }

                return response;
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
