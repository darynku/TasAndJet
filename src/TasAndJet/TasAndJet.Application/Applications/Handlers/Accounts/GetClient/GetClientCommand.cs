using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetClient;

public record GetClientCommand(Guid Id) : IRequest<Result<ProfileResponse, Error>>;