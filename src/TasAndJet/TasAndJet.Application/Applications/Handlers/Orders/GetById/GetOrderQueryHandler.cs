using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using SharedKernel.Common.Exceptions;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.GetById;

public class GetOrderQueryHandler(ApplicationDbContext context, IFileProvider fileProvider)
    : IRequestHandler<GetOrderQuery, Result<OrderDetailsResponse, Error>>
{
    public async Task<Result<OrderDetailsResponse, Error>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            return Errors.General.NotFound(request.OrderId, "Заказ не найден");

        var response = new OrderDetailsResponse
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

        // Дополняем деталями в зависимости от типа заказа
        switch (order.OrderType)
        {
            case OrderType.Rental:
                response.RentalStartDate = order.RentalStartDate;
                response.RentalEndDate = order.RentalEndDate;
                response.ImageUrls = order.ImageKeys.Select(fileProvider.GeneratePreSignedUrl).ToList();
                break;

            case OrderType.Freight:
                response.CargoWeight = order.CargoWeight;
                response.CargoType = order.CargoType;
                break;

            default:
                throw new InvalidOperationException($"Неизвестный тип заказа: {order.OrderType}");
        }

        return Result.Success<OrderDetailsResponse, Error>(response);
    }
}

public class OrderDetailsResponse
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public string Description { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public VehicleType VehicleType { get; set; }
    public string Region { get; set; }
    public decimal TotalPrice { get; set; }
    public KazakhstanCity City { get; set; }

    // Rental
    public DateTime? RentalStartDate { get; set; }
    public DateTime? RentalEndDate { get; set; }
    public List<string>? ImageUrls { get; set; }

    // Freight
    public decimal? CargoWeight { get; set; }
    public string? CargoType { get; set; }
}
