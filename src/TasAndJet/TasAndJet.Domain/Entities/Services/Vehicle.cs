using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Domain.Entities.Services;

public class Vehicle
{
    public Vehicle() { }

    private Vehicle(Guid id, Guid userId, string vehicleType, string mark, double capacity, string? photoUrl)
    {
        Id = id;
        UserId = userId;
        VehicleType = vehicleType;
        Mark = mark;
        Capacity = capacity;
        PhotoUrl = photoUrl;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string VehicleType { get; set; }
    public string Mark { get; set; }
    public double Capacity { get; set; }
    public string? PhotoUrl { get; set; }
    
    public static Vehicle Create(Guid id, Guid userId, string vehicleType, string mark, double capacity, string? photoUrl)
    {
        return new Vehicle(id, userId, vehicleType, mark, capacity, photoUrl);
    }

    
}