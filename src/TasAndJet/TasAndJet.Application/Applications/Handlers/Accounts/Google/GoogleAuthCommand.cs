using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public record GoogleAuthCommand(string GoogleToken) : IRequest<UnitResult<ErrorList>>;