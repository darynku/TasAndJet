using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using TasAndJet.Application.Applications.Handlers.Reviews.Create;

namespace TasAndJet.Api.Controllers;

public class ReviewsController(IMediator mediatr) : ApplicationController
{
    [HttpPost("{orderId:guid}/review")]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand createReviewCommand)
    {
        return Ok();
    }
}