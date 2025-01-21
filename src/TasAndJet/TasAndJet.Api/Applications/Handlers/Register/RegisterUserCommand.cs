using CSharpFunctionalExtensions;
using MediatR;
using TasAndJet.Api.Contracts.Data;
using TasAndJet.Api.Entities;
using TasAndJet.Api.Entities.Account;

namespace TasAndJet.Api.Applications.Handlers.Register;

public class RegisterUserCommand : IRequest<Guid>
{
    public RegisterUserCommand(RegisterData data)
    {
        FirstName = data.FirstName;
        LastName = data.LastName;
        Email = data.Email;
        Password = data.Password;
        PhoneNumber = data.PhoneNumber;
        Region = data.Region;
        Address = data.Address;
        RoleId = data.RoleId;
    }
    
    public string FirstName { get; }
    public string LastName { get;  }
    public string Email { get; }
    public string Password { get;  }
    public string PhoneNumber { get; }
    public string Region { get; }
    public string Address { get; }
    public int RoleId { get; }
}