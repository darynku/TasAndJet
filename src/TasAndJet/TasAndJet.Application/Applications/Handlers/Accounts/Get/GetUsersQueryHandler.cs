using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;
#pragma warning disable CS8601 // Possible null reference assignment.

namespace TasAndJet.Application.Applications.Handlers.Accounts.Get;

public class GetUsersQueryHandler(ApplicationDbContext context) : IRequestHandler<GetUsersQuery, PagedList<UserResponse>>
{
    public async Task<PagedList<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phonenumber = u.PhoneNumber,
                Region = u.Region,
                Address = u.Address,
                Role = u.Role,
                AvatarUrl = u.AvatarUrl
            });

        if (!query.Any())
            return new PagedList<UserResponse>() { };
        
        return await query.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}