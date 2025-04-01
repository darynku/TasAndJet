using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Services.Accounts.UploadFile;

public class UploadFileService(ApplicationDbContext context, IFileProvider fileProvider) : IUploadFileService
{
    public async Task HandleUserAvatar(Guid userId, IFormFile fileRequest, CancellationToken cancellationToken)
    {
        var user = await context.Users
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        if (fileRequest.Length == 0)
            throw new ArgumentException("Файл пуст");

        // Проверка: файл не пустой
        if (fileRequest == null || fileRequest.Length == 0)
            throw new ArgumentException("Файл пуст");

        // Валидация: файл должен быть изображением
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(fileRequest.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Файл должен быть изображением (например, JPEG, PNG, GIF и т.д.)");

        // Валидация: максимальный размер файла (например, 5 МБ)
        const long maxFileSize = 5 * 1024 * 1024; // 5 МБ в байтах
        if (fileRequest.Length > maxFileSize)
            throw new ArgumentException($"Размер файла не должен превышать {maxFileSize / (1024 * 1024)} МБ");
        
        var fileKey = $"users/{user.Id}/{Guid.NewGuid()}{Path.GetExtension(fileRequest.FileName)}";

        await using var stream = fileRequest.OpenReadStream();
        
        var uploadRequest = new UploadFileRequest(
            Key: fileKey,
            FileName: fileRequest.FileName,
            ContentType: fileRequest.ContentType,
            Stream: stream);

        await fileProvider.UploadFileAsync(uploadRequest, cancellationToken);
        user.SetAvatarUrl(fileKey);

        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task HandleVehiclePhoto(Guid vehicleId, IFormFile fileRequest, CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles
                          .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken)
                      ?? throw new NotFoundException("Транспорт не найден");

        if (fileRequest.Length == 0)
            throw new ArgumentException("Файл пуст");

        var fileKey = $"vehicles/{vehicle.Id}/{Guid.NewGuid()}{Path.GetExtension(fileRequest.FileName)}";

        await using var stream = fileRequest.OpenReadStream();
        var uploadRequest = new UploadFileRequest(
            Key: fileKey,
            FileName: fileRequest.FileName,
            ContentType: fileRequest.ContentType,
            Stream: stream);

        await fileProvider.UploadFileAsync(uploadRequest, cancellationToken);

        vehicle.SetPhotoUrl(fileKey);

        await context.SaveChangesAsync(cancellationToken);
    }
}