using TasAndJet.Domain.Entities.Orders;

namespace TasAndJet.Contracts.Data.Orders;

public class ChangeStatusData
{ 
    public required OrderStatus OrderStatus { get; set; }   
}