using Microsoft.AspNetCore.Http;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Data.Accounts;

public class RegisterData
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Region { get; set; }
    public required string Address { get; set; }
    public required int RoleId { get; set; }
    

}