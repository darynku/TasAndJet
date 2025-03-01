using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Login;

public class LoginUserCommand : IRequest<Result<TokenResponse, Error>>
{
    public LoginUserCommand(LoginData data)
    {
        Email = data.Email;
        Password = data.Password;
    }
    public string Email { get; }
    public string Password { get; }
}