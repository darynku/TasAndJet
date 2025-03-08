using Microsoft.EntityFrameworkCore;
using TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Tests.Accounts;

public class RefreshTokenHandlerTests(IntegrationTestWebFactory factory) : TestsBase(factory)
{
    private async Task<(User User, RefreshSession Session)> SeedUserWithRefreshToken(
        string email, 
        int roleId, 
        DateTime? expiresIn = null)
    {
        var user = await SeedUser(email, "password", roleId);

        var refreshToken = Guid.NewGuid();
        var refreshSession = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn ?? DateTime.UtcNow.AddDays(7), // По умолчанию 7 дней
            User = user
        };
        await Context.RefreshSessions.AddAsync(refreshSession);
        await Context.SaveChangesAsync();

        return (user, refreshSession);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsLoginResponse()
    {
        // Arrange
        await SeedRoles();
        var (user, session) = await SeedUserWithRefreshToken("test@example.com", 2); // Роль User

        var command = new RefreshTokenCommand(session.RefreshToken);

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.Value.RefreshToken.ToString()));
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.NotNull(result.Value.Role);
        Assert.Equal(2, result.Value.Role.Id);
        Assert.Equal("User", result.Value.Role.Name);

        // Проверяем, что старый токен удален
        var oldSession = await Context.RefreshSessions
            .FirstOrDefaultAsync(s => s.RefreshToken == session.RefreshToken);
        Assert.Null(oldSession);
    }

    [Fact]
    public async Task Handle_NonExistentRefreshToken_ReturnsNotFoundError()
    {
        // Arrange
        await SeedRoles();
        var nonExistentToken = Guid.NewGuid();
        var command = new RefreshTokenCommand(nonExistentToken);

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Single(result.Error);
        Assert.Equal("refresh.token.not.found", result.Error.Select(x => x.Code).FirstOrDefault());
        Assert.Equal($"Не найден токен: {nonExistentToken}", result.Error.Select(x => x.Message).FirstOrDefault());
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ReturnsExpiredTokenError()
    {
        // Arrange
        await SeedRoles();
        var (user, session) = await SeedUserWithRefreshToken(
            "test@example.com", 
            2, 
            DateTime.UtcNow.AddDays(-1) // Истекший токен
        );

        var command = new RefreshTokenCommand(session.RefreshToken);

        // Act
        var result = await Mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
}