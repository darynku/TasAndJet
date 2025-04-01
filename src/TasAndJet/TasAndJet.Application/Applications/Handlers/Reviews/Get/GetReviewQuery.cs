using MediatR;
using SharedKernel.Paged;
using TasAndJet.Domain.Entities.Reviews;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Get;

public record GetReviewQuery(int Page, int PageSize) : IRequest, IRequest<PagedList<Review>>;