using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Dto;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetDriver;

public class GetDriverCommandHandler(ApplicationDbContext context, IFileProvider fileProvider) : IRequestHandler<GetDriverCommand, Result<DriverProfileResponse, Error>>
{
    public async Task<Result<DriverProfileResponse, Error>> Handle(GetDriverCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(x => x.Id == request.Id)
            .Select(x => new DriverProfileResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Region = x.Region,
                Address = x.Address,
                Role = x.Role,
                AvatarUrl = x.AvatarUrl,
                Orders = x.DriverOrders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    ClientName = o.Client.FirstName + " " + o.Client.LastName,
                    DriverName = o.Driver.FirstName + " " + o.Driver.LastName,
                    Description = o.Description,
                    PickupAddress = o.PickupAddress,
                    DestinationAddress = o.DestinationAddress,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                }).ToList(),
                Reviews = x.Reviews.Select(r => new ReviewDto
                {
                    Comment = r.Comment,
                    Rating = r.Rating
                }).ToList(),
                Vehicles = x.Vehicles.Select(v => new VehicleResponse
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    Capacity = v.Capacity,
                    Mark = v.Mark,
                    Number = v.Number,
                    Colour = v.Colour,
                    PhotoUrl = fileProvider.GeneratePreSignedUrl(v.PhotoUrl),
                    VehicleType = v.VehicleType
                })
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Errors.User.InvalidCredentials();
        }

        if (user.Role.Name != Role.Driver)
        {
            return Errors.User.AccessDenied();
        }
        return Result.Success<DriverProfileResponse, Error>(user);
    }
}