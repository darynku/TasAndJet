using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;

public class RefreshTokenHandler
    (ApplicationDbContext context, IJwtProvider jwtProvider) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse, ErrorList>>
{
    public async Task<Result<LoginResponse, ErrorList>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshSession = await context.RefreshSessions
            .Include(r => r.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken, cancellationToken);
        
        if(oldRefreshSession is null)
            return Error.NotFound("refresh.token.not.found",$"Не найден токен: {request.RefreshToken}").ToErrorList();
        
        if(oldRefreshSession.ExpiresIn < DateTime.UtcNow)
            return Errors.Tokens.ExpiredToken().ToErrorList();
        
        context.RefreshSessions.Remove(oldRefreshSession);
        await context.SaveChangesAsync(cancellationToken);
        
        var accessToken = jwtProvider.GenerateAccessToken(oldRefreshSession.User);
        var refreshToken = await jwtProvider.GenerateRefreshToken(oldRefreshSession.User, cancellationToken);
        
        var role = oldRefreshSession.User.Role;
        
        return new LoginResponse(accessToken, refreshToken, oldRefreshSession.UserId, role);
    }
}