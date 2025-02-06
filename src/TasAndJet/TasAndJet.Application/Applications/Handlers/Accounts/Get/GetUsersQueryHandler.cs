using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Dto;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;

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
                Orders = u.ClientOrders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    ClientName = context.Users.Where(c => c.Id == o.ClientId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault(),
                    DriverName = context.Users.Where(d => d.Id == o.DriverId).Select(d => d.FirstName + " " + d.LastName).FirstOrDefault(),
                    Description = o.Description,
                    PickupAddress = o.PickupAddress,
                    DestinationAddress = o.DestinationAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Service = o.Service
                }).ToList()
            });
        return await query.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}