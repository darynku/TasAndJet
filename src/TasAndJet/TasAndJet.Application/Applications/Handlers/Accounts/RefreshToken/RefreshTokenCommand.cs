using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;

public record RefreshTokenCommand(Guid RefreshToken) : IRequest<Result<LoginResponse, ErrorList>>;