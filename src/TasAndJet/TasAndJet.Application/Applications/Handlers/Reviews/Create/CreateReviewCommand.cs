using CSharpFunctionalExtensions;
using SharedKernel.Common;
using TasAndJet.Contracts.Data.Review;
using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Create;

public class CreateReviewCommand : IRequest<UnitResult<ErrorList>>
{
    public CreateReviewCommand(ReviewData data)
    {
        Id = data.Id;
        ClientId = data.ClientId;
        DriverId = data.DriverId;
        OrderId = data.OrderId;
        Comment = data.Comment;
        Rating = data.Rating;
    }
    public Guid Id { get; }
    public Guid ClientId { get;}
    public Guid DriverId { get; }
    public Guid OrderId { get; }
    public string Comment { get; }
    public int Rating { get; }
}