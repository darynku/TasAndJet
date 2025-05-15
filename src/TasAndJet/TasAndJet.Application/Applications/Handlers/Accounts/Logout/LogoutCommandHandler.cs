using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Logout;

public class LogoutCommandHandler(ApplicationDbContext context) : IRequestHandler<LogoutCommand, UnitResult<ErrorList>>
{
    public async Task<UnitResult<ErrorList>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshSession = await GetByRefreshToken(request.RefreshToken, cancellationToken);
        
        if(oldRefreshSession.IsFailure)
            return oldRefreshSession.Error.ToErrorList();
        
        Delete(oldRefreshSession.Value);
        await context.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
    
    private async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken)
    {
        var refreshSession = await context.RefreshSessions
            .Include(r => r.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(r => r.RefreshToken == refreshToken, cancellationToken);

        if (refreshSession is null)
            return Errors.General.NotFound(refreshToken);

        return refreshSession;
    }

    private void Delete(RefreshSession refreshSession)
    {
        context.RefreshSessions.Remove(refreshSession);
    }

}