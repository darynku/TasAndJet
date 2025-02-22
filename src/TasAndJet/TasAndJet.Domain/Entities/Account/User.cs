using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Reviews;

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
        string? passwordHash,
        string? googleId,
        string phoneNumber,
        string? region,
        string? address,
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
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
    public string? Region { get; private set; }
    public bool PhoneConfirmed { get; private set; }

    public string? AvatarUrl { get; set; }
    public string? GoogleId { get; private set; } // Google ID пользователя

    // Stripe данные
    public string? StripeCustomerId { get; private set; }
    public string? StripePaymentMethodId { get; private set; }
    public string? StripeAccountId { get; private set; }

    private readonly List<Order> _clientOrders = [];
    private readonly List<Order> _driverOrders = [];
    private readonly List<Review> _reviews = [];
    
    public IReadOnlyCollection<Order> ClientOrders => _clientOrders.AsReadOnly();
    public IReadOnlyCollection<Order> DriverOrders => _driverOrders.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    
    
    // 🔹 Фабричный метод для регистрации через email + пароль
    public static User CreateUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string passwordHash,
        string phoneNumber,
        string region,
        string address,
        Role role)
    {
        return new User(id, firstName, lastName, email, passwordHash, null, phoneNumber, region, address, role);
    }

    // 🔹 Фабричный метод для регистрации через Google
    public static User CreateGoogleUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string googleId,
        string phoneNumber,
        Role role) // Пароль отсутствует
    {
        return new User(id, firstName, lastName, email, null, googleId, phoneNumber, null, null, role);
    }
    
    public void LinkGoogleAccount(string googleId)
    {
        if (!string.IsNullOrEmpty(GoogleId))
            throw new InvalidOperationException("Google аккаунт уже привязан");

        GoogleId = googleId;
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
}