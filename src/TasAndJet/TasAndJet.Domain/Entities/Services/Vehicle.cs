using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Domain.Entities.Services;

public class Vehicle
{
    public Vehicle() { }

    private Vehicle(Guid id, Guid userId, VehicleType vehicleType, string mark, string number, string colour, double capacity, string? photoUrl)
    {
        Id = id;
        UserId = userId;
        VehicleType = vehicleType;
        Mark = mark;
        Number = number;
        Colour = colour;
        Capacity = capacity;
        PhotoUrl = photoUrl;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public VehicleType VehicleType { get; set; }
    public string Mark { get; set; }
    public string Number { get; set; }
    public string Colour { get; set; }
    public double Capacity { get; set; }
    public string? PhotoUrl { get; set; }
    
    public static Vehicle Create(Guid id, Guid userId, VehicleType vehicleType, string mark, string number, string colour, double capacity, string? photoUrl)
    {
        return new Vehicle(id, userId, vehicleType, mark, number, colour, capacity, photoUrl);
    }

    public void SetPhotoUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL фото не может быть пустым");

        PhotoUrl = url;
    }
}