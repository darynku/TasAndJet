using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure.Options;
using TasAndJet.Infrastructure.Providers.Abstract;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TasAndJet.Infrastructure.Providers;

public class JwtProvider(IOptions<JwtOptions> jwtOptions, ApplicationDbContext context)
    : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtOptions.Expires));
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
    
    public async Task<Guid> GenerateRefreshToken(User user, CancellationToken cancellationToken)
    {
        var refreshSession = new RefreshSession
        {
            User = user, 
            ExpiresIn = DateTime.UtcNow.AddDays(30), 
            CreatedAt = DateTime.UtcNow,
            RefreshToken = Guid.NewGuid(),
        };

        context.Add(refreshSession);
        await context.SaveChangesAsync(cancellationToken);

        return refreshSession.RefreshToken;
    }

}