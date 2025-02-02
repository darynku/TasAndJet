using MediatR;
using TasAndJet.Api.Contracts.Data.Accounts;
using TasAndJet.Api.Contracts.Response;

namespace TasAndJet.Api.Applications.Handlers.Accounts.Login;

public class LoginUserCommand : IRequest<TokenResponse>
{
    public LoginUserCommand(LoginData data)
    {
        Email = data.Email;
        Password = data.Password;
    }
    public string Email { get; }
    public string Password { get; }
}