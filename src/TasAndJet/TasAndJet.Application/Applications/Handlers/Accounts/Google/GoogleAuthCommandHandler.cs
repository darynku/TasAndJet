using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel.Common;
using GoogleOptions = TasAndJet.Infrastructure.Options.GoogleOptions;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, UnitResult<ErrorList>>
{
    private readonly GoogleOptions _googleOptions;

    public GoogleAuthCommandHandler(IOptions<GoogleOptions> googleOptions)
    {
        _googleOptions = googleOptions.Value;
    }

    public Task<UnitResult<ErrorList>> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
}