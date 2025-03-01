using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Logout;

public record LogoutCommand(Guid RefreshToken) : IRequest<UnitResult<ErrorList>>;