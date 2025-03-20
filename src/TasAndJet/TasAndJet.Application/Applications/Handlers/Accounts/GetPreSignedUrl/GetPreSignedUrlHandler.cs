using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.GetPreSignedUrl;
public record GetPreSignedUrlRequest(Guid Id) : IRequest<string>;

public class GetPreSignedUrlHandler(IFileProvider fileProvider, ApplicationDbContext context) : IRequestHandler<GetPreSignedUrlRequest, string>
{
    public async Task<string> Handle(GetPreSignedUrlRequest request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) 
                   ?? throw new NotFoundException("Пользователь не найден");

        var fileKey = user.AvatarUrl;
        
        if (string.IsNullOrEmpty(fileKey))
            throw new NotFoundException("У пользователя не загружен аватар");
        
        var preSignedUrl = fileProvider.GeneratePreSignedUrl(fileKey);
        
        return preSignedUrl;
    }
}