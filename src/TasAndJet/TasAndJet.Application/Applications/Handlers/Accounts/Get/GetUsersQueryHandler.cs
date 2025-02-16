using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Dto;
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
                Orders = u.ClientOrders.Select(o => new OrderDto
                    {
                        Id = o.Id,
                        ClientName = u.FirstName + " " + u.LastName,
                        DriverName = o.Driver.FirstName + " " + o.Driver.LastName,
                        Description = o.Description,
                        PickupAddress = o.PickupAddress,
                        DestinationAddress = o.DestinationAddress,
                        OrderDate = o.OrderDate,
                        Status = o.Status,
                        Service = o.Service
                    }).ToList()
                    .Concat(
                        u.DriverOrders.Select(o => new OrderDto
                        {
                            Id = o.Id,
                            ClientName = o.Client.FirstName + " " + o.Client.LastName,
                            DriverName = u.FirstName + " " + u.LastName,
                            Description = o.Description,
                            PickupAddress = o.PickupAddress,
                            DestinationAddress = o.DestinationAddress,
                            OrderDate = o.OrderDate,
                            Status = o.Status,
                            Service = o.Service
                        })
                    ).ToList()
            });

        return await query.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}