using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Handlers.Orders.ChangeStatus;
using TasAndJet.Application.Applications.Handlers.Orders.Create;
using TasAndJet.Application.Applications.Handlers.Orders.GetById;
using TasAndJet.Contracts.Data.Orders;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Api.Controllers;

public class OrdersController(
    IMediator mediator,
    IFileProvider fileProvider) : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderData data, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadPhoto(IFormFile file, CancellationToken cancellationToken)
    {
        var request = new UploadFileRequest(Guid.NewGuid().ToString(), file.FileName, file.ContentType, file.OpenReadStream());
        await fileProvider.UploadFileAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrderQuery query, CancellationToken cancellationToken)
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

    [HttpPut("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] Guid orderId, 
        [FromBody] ChangeStatusData data,
        CancellationToken cancellationToken)
    {
        var command = new ChangeStatusCommand(orderId, data);
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        return Ok();
    }
}