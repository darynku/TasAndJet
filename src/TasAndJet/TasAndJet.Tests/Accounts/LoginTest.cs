using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Handlers.Accounts.Login;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Domain.Entities.Account;
namespace TasAndJet.Tests.Accounts;

public class LoginUserCommandHandlerTests(IntegrationTestWebFactory factory) : TestsBase(factory)
{

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokenResponse()
    {
        // Arrange
        await SeedRoles();
        var email = "test@example.com";
        var password = "Password123!";
        await SeedUser(email, password, 2); // Роль User

        var command = new LoginUserCommand(new LoginData
        {
            Email = email,
            Password = password
        });

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.Value.RefreshToken.ToString()));
        Assert.NotNull(result.Value.Role);
        Assert.Equal(2, result.Value.Role.Id);
        Assert.Equal("User", result.Value.Role.Name);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var email = "test@example.com";
        await SeedUser(email, "Password123!", 2); // Роль User

        var command = new LoginUserCommand(new LoginData
        {
            Email = email,
            Password = "WrongPassword" // Неверный пароль
        });

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Неверные учетные данные", result.Error.Message);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ReturnsNotFoundError()
    {
        // Arrange
        var command = new LoginUserCommand(new LoginData
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        });

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("user.not.found", result.Error.Code);
        Assert.Equal("Данного пользователя не существует", result.Error.Message);
    }
    
}
