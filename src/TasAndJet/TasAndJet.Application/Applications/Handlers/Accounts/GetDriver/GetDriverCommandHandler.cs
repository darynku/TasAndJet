﻿using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Dto;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetDriver;

public class GetDriverCommandHandler(ApplicationDbContext context) : IRequestHandler<GetDriverCommand, Result<ProfileResponse, Error>>
{
    public async Task<Result<ProfileResponse, Error>> Handle(GetDriverCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(x => x.Id == request.Id)
            .Select(x => new ProfileResponse
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
                }).ToList()
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
        return Result.Success<ProfileResponse, Error>(user);
    }
}