using MediatR;
using SharedKernel.Common.Exceptions;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Update;

public class UpdateUserProfileRequest : IRequest
{
    public UpdateUserProfileRequest(Guid userId, UpdateData data)
    {
        UserId = userId;
        FirstName = data.FirstName;
        LastName = data.LastName;
        Email = data.Email;
        PhoneNumber = data.PhoneNumber;
        Region = data.Region;
        Address = data.Address;
    }
    public Guid UserId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Region { get; set; }
    public string Address { get; set; }
}

public class UpdateData
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Region { get; set; }
    public required string Address { get; set; }
}