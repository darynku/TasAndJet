using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Accounts.GetClient;
using TasAndJet.Application.Applications.Handlers.Accounts.GetDriver;
using TasAndJet.Application.Applications.Handlers.Accounts.Update;

namespace TasAndJet.Api.Controllers;

[SwaggerTag("Контроллер для работы с профилем водителя и клиента")]
public class ProfilesController(IMediator mediatr) : ApplicationController
{
    [HttpGet("driver/{id:guid}")]
    public async Task<ActionResult> GetDriver([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetDriverCommand(id);
        var result = await mediatr.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Ok(result.Value);
    }
    
    [HttpGet("client/{id:guid}")]
    public async Task<ActionResult> GetClient([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetClientCommand(id);
        var result = await mediatr.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Ok(result.Value);
            
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUserProfile([FromRoute] Guid userId, UpdateData data,
        CancellationToken cancellationToken)
    {
        var request = new UpdateUserProfileRequest(userId, data);
        await mediatr.Send(request, cancellationToken);
        return NoContent();
    }
}