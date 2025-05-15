using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;


namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public record GoogleAuthCommand(string GoogleToken, string PhoneNumber, string Region, string Address, int RoleId) : IRequest<Result<TokenResponse, Error>>;