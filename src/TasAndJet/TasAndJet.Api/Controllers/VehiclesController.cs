using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Orders.UploadFile;
using TasAndJet.Application.Applications.Handlers.Vehicles.Get;
using TasAndJet.Application.Applications.Handlers.Vehicles.GetVehiclePhoto;

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
    
    [HttpGet("{vehicleId}/photo")]
    public async Task<IActionResult> GetVehiclePhoto(Guid vehicleId, CancellationToken cancellationToken)
    {
        var request = new GetVehiclePhotoCommand(vehicleId);
        var preSignedUrl = await mediator.Send(request, cancellationToken);
        return Ok(preSignedUrl);
    }
    [HttpGet]
    public async Task<IActionResult> GetVehicles([FromQuery] GetVehiclesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}