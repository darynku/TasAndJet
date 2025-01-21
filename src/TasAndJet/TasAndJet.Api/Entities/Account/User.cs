using CSharpFunctionalExtensions;

namespace TasAndJet.Api.Entities.Account;

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
        string passwordHash,
        string phoneNumber, 
        string region,
        string address, 
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Region = region;
        Address = address;
        Role = role;
    } 

    public Guid Id { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string Region { get; set; }
    public string Address { get; set; }
    public Role Role { get; set; }
    
    
    //TODO
    public static Result<User> CreateUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string password,
        string phoneNumber,
        string region,
        string address,
        Role role)
    {
        return new User(id, firstName, lastName, email, password, phoneNumber, region, address, role);
    }
}