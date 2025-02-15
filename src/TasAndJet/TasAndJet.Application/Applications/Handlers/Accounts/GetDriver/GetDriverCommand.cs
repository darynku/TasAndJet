using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetDriver;

public record GetDriverCommand(Guid Id) : IRequest<Result<ProfileResponse, Error>>; 