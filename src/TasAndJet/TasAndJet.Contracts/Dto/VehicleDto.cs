namespace TasAndJet.Contracts.Dto;

public class VehicleDto
{
    public string VehicleType { get; init; } = null!;
    public string Mark { get; init; } = null!;
    public double Capacity { get; init; }
    public string? PhotoUrl { get; init; }
}
