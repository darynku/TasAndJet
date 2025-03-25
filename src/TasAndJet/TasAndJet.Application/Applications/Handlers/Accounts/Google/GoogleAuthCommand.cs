using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Dto;
using TasAndJet.Contracts.Response;


namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public record GoogleAuthCommand(string GoogleToken, string PhoneNumber, string Region, string Address, int RoleId, VehicleDto VehicleDto) : IRequest<Result<TokenResponse, Error>>;