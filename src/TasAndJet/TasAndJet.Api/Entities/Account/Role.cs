namespace TasAndJet.Api.Entities.Account;

public class Role 
{
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