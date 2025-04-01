using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Handlers.Accounts.Login;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Domain.Entities.Account;
namespace TasAndJet.Tests.Accounts;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

public class LoginUserCommandHandlerTests : TestsBase
{
    private readonly ILogger<LoginUserCommandHandlerTests> _logger;

    public LoginUserCommandHandlerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _logger = factory.Services.GetRequiredService<ILogger<LoginUserCommandHandlerTests>>();
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokenResponse()
    {
        _logger.LogInformation("Начало теста: Handle_ValidCredentials_ReturnsTokenResponse");

        // Arrange
        await SeedRoles();
        var email = "test@example.com";
        var password = "Password123!";
        await SeedUser(email, password, 2); // Роль User
        _logger.LogDebug("Пользователь {Email} создан с ролью {RoleId}", email, 2);

        var command = new LoginUserCommand(new LoginData
        {
            Email = email,
            Password = password
        });

        // Act
        var result = await Mediator.Send(command);
        _logger.LogDebug("Результат выполнения команды: {@Result}", result);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.Value.RefreshToken.ToString()));
        Assert.NotNull(result.Value.Role);
        Assert.Equal(2, result.Value.Role.Id);
        Assert.Equal("User", result.Value.Role.Name);

        _logger.LogInformation("Тест успешно завершен: Handle_ValidCredentials_ReturnsTokenResponse");
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsInvalidCredentialsError()
    {
        _logger.LogInformation("Начало теста: Handle_InvalidPassword_ReturnsInvalidCredentialsError");

        // Arrange
        var email = "test@example.com";
        await SeedUser(email, "Password123!", 2); // Роль User
        _logger.LogDebug("Пользователь {Email} создан с паролем {Password}", email, "Password123!");

        var command = new LoginUserCommand(new LoginData
        {
            Email = email,
            Password = "WrongPassword" // Неверный пароль
        });

        // Act
        var result = await Mediator.Send(command);
        _logger.LogWarning("Ошибка аутентификации для пользователя {Email}: {@Error}", email, result.Error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Неверные учетные данные", result.Error.Message);

        _logger.LogInformation("Тест завершен: Handle_InvalidPassword_ReturnsInvalidCredentialsError");
    }

    [Fact]
    public async Task Handle_NonExistentUser_ReturnsNotFoundError()
    {
        _logger.LogInformation("Начало теста: Handle_NonExistentUser_ReturnsNotFoundError");

        // Arrange
        var command = new LoginUserCommand(new LoginData
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        });

        // Act
        var result = await Mediator.Send(command);
        _logger.LogWarning("Попытка входа с несуществующим пользователем: {@Command}", command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("user.not.found", result.Error.Code);
        Assert.Equal("Данного пользователя не существует", result.Error.Message);

        _logger.LogInformation("Тест завершен: Handle_NonExistentUser_ReturnsNotFoundError");
    }
}
