using CSharpFunctionalExtensions;
using TasAndJet.Contracts.Data.Review;
using MediatR;
using SharedKernel.Common.Api;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Create;

public class CreateReviewCommand : IRequest<UnitResult<ErrorList>>
{
    public CreateReviewCommand(Guid orderId, ReviewData data)
    {
        OrderId = orderId;
        Comment = data.Comment;
        Rating = data.Rating;
    }
    public Guid OrderId { get; }
    public string Comment { get; }
    public int Rating { get; }
}