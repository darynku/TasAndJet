using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Services.Accounts.UploadFile;

public class UploadFileService(ApplicationDbContext context, IFileProvider fileProvider) : IUploadFileService
{
    private const int MaxOrderCountPhotos = 6;
    private const long MaxFileSize = 20 * 1024 * 1024;
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    public async Task HandleUserAvatar(Guid userId, IFormFile fileRequest, CancellationToken cancellationToken)
    {
        var user = await context.Users
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");
        
        ValidateFileRequest(fileRequest);
        
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
        
        ValidateFileRequest(fileRequest);
        
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
    
    public async Task HandleOrderPhotos(Guid orderId, IFormFileCollection files, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(u => u.Id == orderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        if (files == null || !files.Any())
            throw new ArgumentException("Файлы не выбраны");

        if (files.Count >= MaxOrderCountPhotos)
            throw new ArgumentException("Слишком много файлов, максимальное количество");
        
        foreach (var fileRequest in files)
        {
            ValidateFileRequest(fileRequest);
            
            var fileKey = $"orders/rental/{order.Id}/{Guid.NewGuid()}{Path.GetExtension(fileRequest.FileName)}";

            await using var stream = fileRequest.OpenReadStream();
        
            var uploadRequest = new UploadFileRequest(
                Key: fileKey,
                FileName: fileRequest.FileName,
                ContentType: fileRequest.ContentType,
                Stream: stream);

            await fileProvider.UploadFileAsync(uploadRequest, cancellationToken);

            order.AddImageKey(fileKey);
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    private void ValidateFileRequest(IFormFile fileRequest)
    {
        if (fileRequest == null || fileRequest.Length == 0)
            throw new ArgumentException("Файл пуст");
        
        var fileExtension = Path.GetExtension(fileRequest.FileName).ToLowerInvariant();
        
        if (!_allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Файл должен быть изображением (например, JPEG, PNG, GIF и т.д.)");
        
        if (fileRequest.Length > MaxFileSize)
            throw new ArgumentException($"Размер файла не должен превышать {MaxFileSize / (1024 * 1024)} МБ");
    }
}