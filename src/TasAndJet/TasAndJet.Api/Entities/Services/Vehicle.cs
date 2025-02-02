namespace TasAndJet.Api.Entities.Services;

public class Vehicle
{
    private Vehicle() { }

    private Vehicle(Guid id, string vehicleType, string? photoUrl)
    {
        Id = id;
        VehicleType = vehicleType;
        PhotoUrl = photoUrl;
    }
    public Guid Id { get; set; }
    public string VehicleType { get; set; }
    public string? PhotoUrl { get; set; }
    
    public static Vehicle Create(Guid id, string vehicleType, string? photoUrl)
    {
        return new Vehicle(id, vehicleType, photoUrl);
    }

    
}