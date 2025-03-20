namespace TasAndJet.Domain.Entities.Account;

public class Role 
{
    public const string Driver = "Driver";
    public const string Client = "Client";
    private Role()
    {
    }
    public Role(int roleId, string roleName) 
    {
        Id = roleId;
        Name = roleName;
    } 
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}