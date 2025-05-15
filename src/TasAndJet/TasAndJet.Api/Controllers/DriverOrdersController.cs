using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Orders.CreateDriverRental;
using TasAndJet.Application.Applications.Handlers.Orders.Drivers;
using TasAndJet.Application.Applications.Handlers.Orders.Drivers.CancelByUser;
using TasAndJet.Application.Applications.Handlers.Orders.Drivers.CompleteByUser;
using TasAndJet.Application.Applications.Handlers.Orders.Drivers.ConfirmByClient;
using TasAndJet.Application.Applications.Handlers.Orders.Drivers.GetById;
using TasAndJet.Application.Applications.Handlers.Orders.Get;

namespace TasAndJet.Api.Controllers;

[SwaggerTag("Контроллер для работы с заказами от водителя")]
public class DriverOrdersController(IMediator mediator) : ApplicationController
{
    /// <summary>
    /// Создание нового заказа аренды водителем
    /// </summary>
    [HttpPost("rental")]
    public async Task<IActionResult> CreateRentalOrder([FromForm] CreateDriverRentalCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Получение списка заказов для водителей
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetDriverOrders([FromQuery] GetDriversOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Получение детальной информации о заказе по идентификатору
    /// </summary>
    [HttpGet("{orderId:guid}")]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await mediator.Send(new GetByIdDriverOrderQuery(orderId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    /// <summary>
    /// Назначение водителя на заказ
    /// </summary>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignDriver([FromBody] AssignByUseCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
    
    /// <summary>
    /// Подтверждение заказа клиентом
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmByClientCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
    
    /// <summary>
    /// Завершение заказа водителем
    /// </summary>
    [HttpPost("complete")]
    public async Task<IActionResult> CompleteOrder([FromBody] CompleteByUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
    
    /// <summary>
    /// Отмена заказа водителем
    /// </summary>
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelByUserCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}