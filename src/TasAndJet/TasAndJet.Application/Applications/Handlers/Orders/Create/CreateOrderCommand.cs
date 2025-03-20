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
        Description = data.Description;
        PickupAddress = data.PickupAddress;
        DestinationAddress = data.DestinationAddress;
        OrderDate = data.OrderDate;
        TotalPrice = data.TotalPrice;
        VehicleType = data.VehicleType;
    }
    public Guid ClientId { get;}
    
    public string Description { get; } 
    public string PickupAddress { get; }
    public string DestinationAddress  { get; }
    
    public DateTime OrderDate { get; }
    public decimal TotalPrice { get; }
    
    public VehicleType VehicleType { get; }

}