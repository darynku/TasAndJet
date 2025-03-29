using Microsoft.AspNetCore.Http;
using TasAndJet.Application.Applications.Handlers.Accounts.Register;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Tests.Accounts;

public class RegisterTest(IntegrationTestWebFactory factory) : TestsBase(factory)
{
    [Fact]
    public async Task Register_Should_Be_True_When_Role_Dont_Exist()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var avatar = new FormFile(Stream.Null, 10, 10, "test.jpg", "test.jpg");
        var vehiclePhoto = new FormFile(Stream.Null, 10, 10, "test.jpg", "test.jpg");
        await SeedRoles();

        var command = new RegisterUserCommand(new RegisterData
        {
            Address = "123 Main Street",
            Email = "test@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            Avatar = avatar,
            Password = "password",
            PhoneNumber = "+77784928492",
            Region = "asdada",
            RoleId = 9, // Ролей всего 3
        });
        
        // Act
        var result = await Mediator.Send(command, cancellationToken);

        // Assert
        Assert.False(result.IsSuccess); 
        Assert.Contains(result.Error!, e => e.Message.Contains("role")); 
    }
}
