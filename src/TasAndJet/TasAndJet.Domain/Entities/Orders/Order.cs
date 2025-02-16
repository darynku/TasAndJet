using CSharpFunctionalExtensions;
using SharedKernel.Common;
using SharedKernel.Domain;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Domain.Events;

namespace TasAndJet.Domain.Entities.Orders;

public class Order : DomainEntity
{
    private Order()
    {
    }

    private Order(
        Guid id,
        Guid clientId,
        Guid driverId,
        string description,
        string pickupAddress,
        string destinationAddress,
        DateTime orderDate,
        Service service)
    {
        Id = id;
        ClientId = clientId;
        DriverId = driverId;
        Description = description;
        PickupAddress = pickupAddress;
        DestinationAddress = destinationAddress;
        OrderDate = orderDate;
        Status = OrderStatus.Created;
        Service = service;
    }


    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid DriverId { get; set; }
    public User Client { get; set; }
    public User Driver { get; set; }


    public string Description { get; set; }
    public string PickupAddress { get; set; }
    public string DestinationAddress { get; set; }

    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; }
    public Service Service { get; set; }

    public string Amount  { get; set; }
    public decimal CheckOut { get; set; }
    public Review? Review { get; set; }

    public string? StripePaymentIntentId { get; set; } // ID платежа
    public string? StripeTransferId { get; set; } // ID перевода водителю
    

    public static Order Create(
        Guid id,
        Guid clientId,
        Guid driverId,
        string description,
        string pickupAddress,
        string destinationAddress,
        DateTime orderDate,
        Service service)
    {
        return new Order(
            id,
            clientId,
            driverId,
            description,
            pickupAddress,
            destinationAddress,
            orderDate,
            service);
    }
    public void AddReview(Review review)
    {
        Review = review;
    }

    public void AssignDriverToOrder(Order order)
    {
        Status = OrderStatus.Assigned;
    }

    public UnitResult<Error> ConfirmOrder(Guid driverId)
    {
        if (DriverId != driverId)
            return Errors.User.InvalidCredentials();
        
        if(Status != OrderStatus.Assigned)
            return Errors.General.ValueIsInvalid("order.assigned.status");
        
        Status = OrderStatus.Confirmed;
        
        AddDomainEvent(new OrderConfirmedEvent(Id));
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> CompleteOrder(Guid driverId)
    {
        if (DriverId != driverId)
            return Errors.User.InvalidCredentials();
        
        if(Status != OrderStatus.Confirmed)
            return Errors.General.ValueIsInvalid("order.confirmed.status");
        
        Status = OrderStatus.Completed;
        
        AddDomainEvent(new OrderCompletedEvent(Id));
        
        return UnitResult.Success<Error>();
    }
    
    public UnitResult<Error> ChangeStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Completed)
            return Errors.Orders.InvalidStatus();

        if (Status == OrderStatus.Canceled)
            return Errors.Orders.CantCancel();
        
        if (newStatus == OrderStatus.Canceled)
        {
            Status = OrderStatus.Canceled;
            return Result.Success<Error>();
        }
        Status = newStatus;
        return Result.Success<Error>();
    }


}