using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Api.Contracts.Data.Accounts;

namespace TasAndJet.Api.Applications.Handlers.Accounts.Register;

public class RegisterUserCommand : IRequest<UnitResult<ErrorList>>
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