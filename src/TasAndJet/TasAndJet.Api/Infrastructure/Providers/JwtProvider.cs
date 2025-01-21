using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TasAndJet.Api.Entities;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Infrastructure.Options;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TasAndJet.Api.Infrastructure.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly ApplicationDbContext _context;
    
    public JwtProvider(IOptions<JwtOptions> jwtOptions, ApplicationDbContext context)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim("Id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        ];

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
            User = user, ExpiresIn = DateTime.UtcNow.AddDays(30), CreatedAt = DateTime.UtcNow, RefreshToken = Guid.NewGuid(),
        };

        _context.Add(refreshSession);
        await _context.SaveChangesAsync(cancellationToken);

        return refreshSession.RefreshToken;
    }

}