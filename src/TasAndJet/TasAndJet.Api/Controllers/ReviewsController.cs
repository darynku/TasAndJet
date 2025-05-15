using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Api;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Application.Applications.Handlers.Reviews.Create;
using TasAndJet.Contracts.Data.Review;

namespace TasAndJet.Api.Controllers;

[SwaggerTag("Контроллер для работы с отзывами под заказом")]
public class ReviewsController(IMediator mediatr) : ApplicationController
{
    [HttpPost("create/{orderId:guid}")]
    public async Task<IActionResult> CreateReview([FromRoute] Guid orderId, [FromBody] ReviewData data, CancellationToken cancellationToken)
    {
        var request = new CreateReviewCommand(orderId, data);
        var result = await mediatr.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Created();
    }
}