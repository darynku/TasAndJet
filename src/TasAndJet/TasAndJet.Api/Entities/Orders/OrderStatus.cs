namespace TasAndJet.Api.Entities.Orders;

public enum OrderStatus
{
    Created,   // Заказ только что создан
    Assigned,  // Исполнитель назначен
    Confirmed, // Заказ подтверждён исполнителем
    Completed, // Заказ завершён
    Canceled   // Заказ отменён
}