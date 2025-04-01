using MediatR;
using SharedKernel.Paged;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Get;

public class GetReviewQueryHandler(ApplicationDbContext context) : IRequestHandler<GetReviewQuery, PagedList<Review>>
{
    public async Task<PagedList<Review>> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    { 
        var reviews = await context.Reviews
            .OrderByDescending(r => r.Rating)
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
        
        return reviews;
    }
}