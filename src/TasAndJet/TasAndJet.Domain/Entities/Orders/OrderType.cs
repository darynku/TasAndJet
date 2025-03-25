using System.ComponentModel;

namespace TasAndJet.Domain.Entities.Orders;

public enum OrderType
{
    [Description("Аренда")]
    Rental = 1,
    
    [Description("Грузоперевозка")]
    Freight = 2 
}