using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Dto;

public class VehicleDto
{
    public VehicleType VehicleType { get; init; }
    public string Mark { get; init; } = null!;
    public double Capacity { get; init; }
    public string? PhotoUrl { get; init; }
}
