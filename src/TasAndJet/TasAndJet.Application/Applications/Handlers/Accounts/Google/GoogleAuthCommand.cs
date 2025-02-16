using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public record GoogleAuthCommand(string GoogleToken, int RoleId) : IRequest<Result<TokenResponse, Error>>;