using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;

public record RefreshTokenCommand(Guid RefreshToken) : IRequest<Result<LoginResponse, ErrorList>>;