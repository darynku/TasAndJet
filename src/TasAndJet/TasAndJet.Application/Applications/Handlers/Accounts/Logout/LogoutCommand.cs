using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Logout;

public record LogoutCommand(Guid RefreshToken) : IRequest<UnitResult<ErrorList>>;