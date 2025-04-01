using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Update;

public class UpdateUserProfileHandler(ApplicationDbContext context, IValidator<UpdateUserProfileRequest> validator) : IRequestHandler<UpdateUserProfileRequest>
{
    public async Task Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        
        await context.Users
            .Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.FirstName, request.FirstName)
                    .SetProperty(u => u.LastName, request.LastName)
                    .SetProperty(u => u.Email, request.Email)
                    .SetProperty(u => u.PhoneNumber, request.PhoneNumber)
                    .SetProperty(u => u.Region, request.Region)
                    .SetProperty(u => u.Address, request.Address),
                cancellationToken);
    }
}