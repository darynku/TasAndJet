using System.ComponentModel;

namespace TasAndJet.Domain.Entities.Orders;

public enum OrderStatus
{
    [Description("Заказ только что создан")]
    Created = 1,
    
    [Description("Исполнитель назначен")]
    Assigned = 2,
    
    [Description("Заказ подтверждён исполнителем")]
    Confirmed = 3, 
    
    [Description("Заказ завершён")]
    Completed = 4, 
    
    [Description("Заказ отменён")]
    Canceled = 5    
}