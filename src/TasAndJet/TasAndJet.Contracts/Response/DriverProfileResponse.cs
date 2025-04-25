using TasAndJet.Contracts.Dto;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Contracts.Response;

public class DriverProfileResponse
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Region { get; set; }
    public required string Address { get; set; }
    public required Role Role { get; set; }
    public string AvatarUrl { get; set; }
    public required IEnumerable<OrderDto> Orders { get; set; }
    public required IEnumerable<ReviewDto> Reviews { get; set; }
    public required IEnumerable<VehicleResponse> Vehicles { get; set; }
}
