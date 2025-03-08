using TasAndJet.Application.Applications.Handlers.Accounts.Register;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Tests.Accounts;

public class RegisterTest(IntegrationTestWebFactory factory) : TestsBase(factory)
{
    [Fact]
    public async Task Register_Should_Be_True_When_Role_Dont_Exist()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        await SeedRoles();

        var command = new RegisterUserCommand(new RegisterData
        {
            Address = "123 Main Street",
            Email = "test@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "password",
            PhoneNumber = "+77784928492",
            Region = "asdada",
            RoleId = 9, // Ролей всего 3
            Mark = "Brbrbrb",
            Capacity = 90.0,
            VehicleType = "Dadada",
            PhotoUrl = "qwerty.png"
        });
        
        // Act
        var result = await Mediator.Send(command, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess); 
        Assert.Contains(result.Error!, e => e.Message.Contains("role")); 
    }
}
