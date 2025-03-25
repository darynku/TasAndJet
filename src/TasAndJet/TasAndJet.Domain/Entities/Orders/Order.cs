using CSharpFunctionalExtensions;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using SharedKernel.Domain;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Domain.Events;

namespace TasAndJet.Domain.Entities.Orders;

public class Order 
{
    private Order()
    {
    }

    private Order(Guid id, Guid clientId, string description, string pickupAddress, string destinationAddress, 
        decimal totalPrice, DateTime orderDate, OrderStatus orderStatus, VehicleType vehicleType, OrderType orderType, string region)
    {
        Id = id;
        ClientId = clientId;
        Description = description;
        PickupAddress = pickupAddress;
        DestinationAddress = destinationAddress;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Created;
        TotalPrice = totalPrice;
        OrderDate = orderDate;
        Status = orderStatus;
        VehicleType = vehicleType;
        OrderType = orderType;
        Region = region;
    }


    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? DriverId { get; private set; } // Водитель назначается позже

    public User Client { get; private set; }
    public User? Driver { get; private set; } // Может быть null, пока не назначен водитель

    public string Description { get; private set; }
    public string PickupAddress { get; private set; }
    public string DestinationAddress { get; private set; }

    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public OrderType OrderType { get; private set;  }
    public VehicleType VehicleType { get; private set; }
    public string Region { get; private set; }
    public decimal TotalPrice { get; private set; } // Итоговая цена заказа

    public Review? Review { get; private set; } // Отзыв на заказ

    public static Order Create(
        Guid id,
        Guid clientId, 
        string description,
        string pickupAddress,
        string destinationAddress,
        decimal totalPrice,
        VehicleType vehicleType,
        OrderType orderType,
        string region)
    {
        return new Order(id, clientId,
            description, 
            pickupAddress, 
            destinationAddress, 
            totalPrice, 
            DateTime.UtcNow, 
            OrderStatus.Created, 
            vehicleType, 
            orderType, 
            region);
    }
    
    public void AddReview(Review review)
    {
        Review = review;
    }
    
    public void AssignDriver(Guid driverId)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Водителя можно назначить только на новый заказ.");

        DriverId = driverId;
        Status = OrderStatus.Assigned;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Assigned)
            throw new InvalidOperationException("Заказ можно подтвердить только после назначения водителя.");

        Status = OrderStatus.Confirmed;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Заказ можно завершить только после подтверждения.");

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Нельзя отменить завершённый заказ.");

        Status = OrderStatus.Canceled;
    }

}