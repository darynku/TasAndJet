using MediatR;
using TasAndJet.Contracts.Data.Orders;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.Create;

public class CreateOrderCommand : IRequest<Guid>
{
    public CreateOrderCommand(OrderData data)
    {
        ClientId = data.ClientId;
        DriverId = data.DriverId;
        Description = data.Description;
        PickupAddress = data.PickupAddress;
        DestinationAddress = data.DestinationAddress;
        OrderDate = data.OrderDate;
        Status = data.Status;
        Title = data.Title;
        Cost = data.Cost;
        ServiceType = data.ServiceType;
        VehicleType = data.VehicleType;
        PhotoUrl = data.PhotoUrl;
    }
    public Guid ClientId { get;}
    public Guid DriverId { get; }
    
    public string Description { get; } 
    public string PickupAddress { get; }
    public string DestinationAddress  { get; }
    
    public DateTime OrderDate { get; }
    public OrderStatus Status { get; }
    
    public string Title { get; } 
    public decimal Cost { get; }
    public string VehicleType { get; }
    public string? PhotoUrl { get; }
    public ServiceType ServiceType { get; }

}