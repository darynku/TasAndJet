using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;

public record UploadUserPhotoCommand(Guid UserId, IFormFile File) : IRequest;

public class UploadUserPhotoCommandHandler(
    ApplicationDbContext context,
    IFileProvider fileProvider) : IRequestHandler<UploadUserPhotoCommand>
{
    private const string Bucket = "tasandjet"; // Имя бакета MinIO

    public async Task Handle(UploadUserPhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
                       .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                            ?? throw new NotFoundException("Пользователь не найден");

        var file = request.File;
        if (file.Length == 0)
            throw new ArgumentException("Файл пуст");

        var fileKey = $"users/{user.Id}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        await using var stream = file.OpenReadStream();
        var uploadRequest = new UploadFileRequest(
            Key: fileKey,
            FileName: file.FileName,
            ContentType: file.ContentType,
            Stream: stream);

        await fileProvider.UploadFileAsync(uploadRequest, cancellationToken);
        user.SetAvatarUrl(fileKey);

        await context.SaveChangesAsync(cancellationToken);
    }
}

