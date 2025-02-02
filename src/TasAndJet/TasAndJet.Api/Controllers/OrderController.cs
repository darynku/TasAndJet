using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasAndJet.Api.Applications.Handlers.Orders.Create;
using TasAndJet.Api.Contracts.Data.Orders;
using TasAndJet.Api.Infrastructure.Providers;
using TasAndJet.Api.Infrastructure.Providers.Abstract;

namespace TasAndJet.Api.Controllers;

public class OrderController(
    IMediator mediator,
    IFileProvider fileProvider) : ApplicationController
{
    [HttpPost("create")]
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
}