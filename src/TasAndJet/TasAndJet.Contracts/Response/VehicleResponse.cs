using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Response;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public VehicleType VehicleType { get; set; }
    public required string Mark { get; set; }
    public required string Number { get; set; }
    public required string Colour { get; set; }
    public double Capacity { get; set; }
    public string? PhotoUrl { get; set; }

}