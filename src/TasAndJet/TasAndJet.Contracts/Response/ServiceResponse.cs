using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Response;

public class ServiceResponse
{
    public string Title { get; set; } 
    public decimal Cost { get; set; }
    public string VehicleType { get; set; }
    public string? PhotoUrl { get; set; }
    public ServiceType ServiceType { get; set; }
}