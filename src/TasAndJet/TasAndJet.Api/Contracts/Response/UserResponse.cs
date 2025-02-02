using TasAndJet.Api.Contracts.Dto;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Entities.Orders;

namespace TasAndJet.Api.Contracts.Response;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phonenumber { get; set; }
    public string Region { get; set; }
    public string Address { get; set; }
    public Role Role { get; set; }
    public IEnumerable<OrderDto> Orders { get; set; }
}