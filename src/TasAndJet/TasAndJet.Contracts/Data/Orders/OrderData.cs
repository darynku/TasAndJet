﻿using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Data.Orders;

public class OrderData
{
    public Guid ClientId { get; set; }
    public Guid DriverId { get; set; }
    public required string Description { get; set; } 
    public required string PickupAddress { get; set; }
    public required string DestinationAddress  { get; set; }
    
    public required DateTime OrderDate { get; set; }
    
    public required OrderStatus Status { get; set; }
    
    public required string Title { get; set; } 

    public required decimal Cost { get; set; }
    public  required ServiceType ServiceType { get; set; }
    
    public required string VehicleType { get; set; }
    public required string Mark { get; set; }
    public required double Capacity { get; set; }
    public required string? PhotoUrl { get; set; }
}