using Microsoft.AspNetCore.Http;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Contracts.Dto;

public class VehicleDto
{
    public string Mark { get; }
    public VehicleType VehicleType { get; }
    public double Capacity { get; }
    public IFormFile? PhotoUrl { get; set; }
}
