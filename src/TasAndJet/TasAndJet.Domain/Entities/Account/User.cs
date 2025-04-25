using CSharpFunctionalExtensions;
using SharedKernel.Common.Api;
using SharedKernel.Common.Exceptions;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Domain.Entities.Account;

public class User 
{
    private User()
    {
    }
    private User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string avatarUrl,
        string? passwordHash,
        string? googleId,
        string phoneNumber,
        string region,
        string address,
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AvatarUrl = avatarUrl;
        PasswordHash = passwordHash;
        GoogleId = googleId;
        PhoneNumber = phoneNumber;
        Region = region;
        Address = address;
        Role = role;
        PhoneConfirmed = false;
    }
 

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } 
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public string? PasswordHash { get; private set; } // Nullable, так как у Google-пользователей нет пароля
    public Role Role { get; private set; }
    public string? Address { get; private set; }
    public string Region { get; private set; }
    public bool PhoneConfirmed { get; private set; }

    public string? AvatarUrl { get; private set; }
    public string? GoogleId { get; private set; } // Google ID пользователя

    // Stripe данные
    public string? StripeCustomerId { get; private set; }
    public UserSubscription UserSubscription { get; set; }

    private readonly List<Order> _clientOrders = [];
    private readonly List<Order> _driverOrders = [];
    private readonly List<Review> _reviews = [];
    private readonly List<Vehicle> _vehicles = [];
    
    public IReadOnlyCollection<Order> ClientOrders => _clientOrders.AsReadOnly();
    public IReadOnlyCollection<Order> DriverOrders => _driverOrders.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();
    
    public static bool HasActiveSubscription(UserSubscription? subscription)
    {
        return subscription != null && subscription.IsPremium();
    }
    // 🔹 Фабричный метод для регистрации через email + пароль
    public static User CreateUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string avatarUrl,
        string passwordHash,
        string phoneNumber,
        string region,
        string address,
        Role role)
    {
        return new User(id, firstName, lastName, email, avatarUrl, passwordHash, null, phoneNumber, region, address, role);
    }

    //Фабричный метод для регистрации через Google
    public static User CreateGoogleUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string avatarUrl,
        string googleId,
        string phoneNumber,
        string region,
        string address,
        Role role) // Пароль отсутствует
    {
        return new User(id, firstName, lastName, email, avatarUrl,null, googleId, phoneNumber, region, address, role);
    }

    public Order CreateRentalOrder(
        Guid id,
        string description,
        DateTime rentalStartDate,
        DateTime rentalEndDate,
        decimal totalPrice,
        VehicleType vehicleType,
        KazakhstanCity city)
    {
        return Order.CreateRentalOrder(id, Id, description, rentalStartDate, rentalEndDate, totalPrice, vehicleType, city, Region);
    }    
    
    public Order CreateFreightOrder(
        Guid id,
        string description,
        string pickupAddress,
        string destinationAddress,
        decimal cargoWeight,
        string cargoType,
        decimal totalPrice,
        VehicleType vehicleType,
        KazakhstanCity city)
    {
        return Order.CreateFreightOrder(id, Id, description, pickupAddress, destinationAddress, cargoWeight, cargoType, totalPrice, vehicleType, city, Region);
    }
    
    public void LinkGoogleAccount(string googleId)
    {
        if (!string.IsNullOrEmpty(GoogleId))
            throw new InvalidOperationException("Google аккаунт уже привязан");

        GoogleId = googleId;
    }

    public void AddSubscription(UserSubscription subscription)
    {
        UserSubscription = subscription;
    }

    public void AddReview(Review review)
    {
        _reviews.Add(review);
    }
    public void AddClientOrder(Order order)
    {
        _clientOrders.Add(order);
    }

    public void AddDriverOrder(Order order)
    {
        _driverOrders.Add(order);
    }
    public void ConfirmPhone()
    {
        PhoneConfirmed = true;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        _vehicles.Add(vehicle);
    }
    
    public void SetStripeCustomerId(string customerId)
    {
        if (string.IsNullOrEmpty(StripeCustomerId))
            StripeCustomerId = customerId;
    }


    
    public void SetAvatarUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL фото не может быть пустым");

        AvatarUrl = url;
    }

}
