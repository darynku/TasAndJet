﻿using CSharpFunctionalExtensions;
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

    public void AddReview(Review review)
    {
        Review = review;
    }

}