using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Handlers.Reviews.Create;
using TasAndJet.Contracts.Data.Review;

namespace TasAndJet.Api.Controllers;

public class ReviewsController(IMediator mediatr) : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateReview([FromBody] ReviewData data, CancellationToken cancellationToken)
    {
        var request = new CreateReviewCommand(data);
        var result = await mediatr.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Created();
    }
}