using TasAndJet.Contracts.Dto;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Contracts.Response;

public class ProfileResponse
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
    public IEnumerable<ReviewDto> Reviews { get; set; }
}