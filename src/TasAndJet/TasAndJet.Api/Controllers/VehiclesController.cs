using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Orders.GetVehiclePhoto;
using TasAndJet.Application.Applications.Handlers.Orders.UploadFile;

namespace TasAndJet.Api.Controllers;

[SwaggerTag("Контроллер для работы с траспортом водителя")]
public class VehiclesController(IMediator mediator) : ApplicationController
{
    [HttpPost("{vehicleId}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(Guid vehicleId, IFormFile file, CancellationToken cancellationToken)
    {
        await mediator.Send(new UploadVehiclePhotoCommand(vehicleId, file), cancellationToken);
        return Ok();
    }
    
    [HttpGet("{vehicleId}/avatar")]
    public async Task<IActionResult> GetAvatar(Guid userId, CancellationToken cancellationToken)
    {
        var request = new GetVehiclePhotoCommand(userId);
        var preSignedUrl = await mediator.Send(request, cancellationToken);
        return Ok(preSignedUrl);
    }
}