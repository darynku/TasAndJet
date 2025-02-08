using CSharpFunctionalExtensions;
using SharedKernel.Common;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Domain.Entities.Orders;

public class Order
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
        OrderStatus status,
        Service service)
    {
        Id = id;
        ClientId = clientId;
        DriverId = driverId;
        Description = description;
        PickupAddress = pickupAddress;
        DestinationAddress = destinationAddress;
        OrderDate = orderDate;
        Status = status;
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

    public Review? Review { get; set; }

    public static Order Create(
        Guid id,
        Guid clientId,
        Guid driverId,
        string description,
        string pickupAddress,
        string destinationAddress,
        DateTime orderDate,
        OrderStatus status,
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
            status,
            service);
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

        if (!IsValidTransition(Status, newStatus))
            return Errors.Orders.InvalidStatus();

        Status = newStatus;
        return Result.Success<Error>();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return (current == OrderStatus.Created && next == OrderStatus.Assigned) ||
               (current == OrderStatus.Assigned && next == OrderStatus.Confirmed) ||
               (current == OrderStatus.Confirmed && next == OrderStatus.Completed);
    }

    public void AddReview(Review review)
    {
        Review = review;
    }

}