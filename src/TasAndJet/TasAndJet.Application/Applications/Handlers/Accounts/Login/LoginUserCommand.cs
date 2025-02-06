using MediatR;
using TasAndJet.Contracts.Data.Accounts;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Login;

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