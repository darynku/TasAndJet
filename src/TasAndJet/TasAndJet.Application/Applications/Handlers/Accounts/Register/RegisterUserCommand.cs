using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Register;

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
        Mark = data.Mark;
        VehicleType = data.VehicleType;
        Capacity = data.Capacity;
        PhotoUrl = data.PhotoUrl;
    }
    
    public string FirstName { get; }
    public string LastName { get;  }
    public string Email { get; }
    public string Password { get;  }
    public string PhoneNumber { get; }
    public string Region { get; }
    public string Address { get; }
    public int RoleId { get; }
    
    public string Mark { get; }
    public VehicleType VehicleType { get; }
    public double Capacity { get; }
    public string? PhotoUrl { get; set; }
}