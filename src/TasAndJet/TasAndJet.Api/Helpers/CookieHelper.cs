using System.Security.Claims;
using CSharpFunctionalExtensions;
using SharedKernel.Common.Api;

namespace TasAndJet.Api.Helpers;

public class CookieHelper(IHttpContextAccessor httpContextAccessor)
{
    private const string RefreshToken = "refreshToken";

    public Result<Guid, Error> GetUserId()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return Errors.General.Failure();
        }

        var userId = httpContextAccessor.HttpContext.User.FindFirstValue("Id");

        if (userId == null)
            return Errors.User.InvalidCredentials();

        return Guid.Parse(userId);
    }

    public Result<Guid, Error> GetRefreshSessionCookie()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return Errors.General.Failure();
        }

        if (!httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(RefreshToken, out var refreshToken))
        {
            return Errors.General.NotFound(null, RefreshToken);
        }

        return Guid.Parse(refreshToken);
    }

    public UnitResult<Error> SetRefreshSessionCookie(Guid refreshToken)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return Errors.General.Failure();
        }

        httpContextAccessor.HttpContext.Response.Cookies.Append(RefreshToken, refreshToken.ToString());

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DeleteRefreshSessionCookie()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return Errors.General.Failure();
        }

        httpContextAccessor.HttpContext.Response.Cookies.Delete(RefreshToken);

        return UnitResult.Success<Error>();
    }

}