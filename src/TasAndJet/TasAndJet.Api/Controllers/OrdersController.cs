using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Api;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Orders.Assign;
using TasAndJet.Application.Applications.Handlers.Orders.Cancel;
using TasAndJet.Application.Applications.Handlers.Orders.Complete;
using TasAndJet.Application.Applications.Handlers.Orders.Confirm;
using TasAndJet.Application.Applications.Handlers.Orders.CreateFreight;
using TasAndJet.Application.Applications.Handlers.Orders.CreateRental;
using TasAndJet.Application.Applications.Handlers.Orders.Get;
using TasAndJet.Application.Applications.Handlers.Orders.GetById;
namespace TasAndJet.Api.Controllers;


[SwaggerTag("Контроллер для работы с заказами")]
public class OrdersController(IMediator mediator) : ApplicationController
{
    [HttpPost("rental")]
    public async Task<IActionResult> CreateRentalOrder([FromForm] CreateRentalOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpPost("freight")]
    public async Task<IActionResult> CreateFreightOrder([FromForm] CreateFreightOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return Ok(id);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("client")]
    public async Task<IActionResult> Client([FromQuery] ClientQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var query = new GetOrderQuery(orderId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(Envelope.Error(result.Error));

        return Ok(result.Value);
    }
    
    [HttpPost("{orderId}/assign-driver/{driverId}")]
    public async Task<IActionResult> AssignDriver(Guid orderId, Guid driverId, CancellationToken cancellationToken)
    {
        await mediator.Send(new AssignDriverCommand(orderId, driverId), cancellationToken);
        return Ok();
    }

    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmOrderCommand(orderId), cancellationToken);
        return Ok();
    }

    [HttpPost("{orderId}/complete")]
    public async Task<IActionResult> CompleteOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await mediator.Send(new CompleteOrderCommand(orderId), cancellationToken);
        return Ok();
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelOrderCommand(orderId), cancellationToken);
        return Ok();
    }
}
