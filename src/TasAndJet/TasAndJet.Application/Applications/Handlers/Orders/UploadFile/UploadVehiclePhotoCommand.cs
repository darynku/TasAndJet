using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Orders.UploadFile;

public record UploadVehiclePhotoCommand(Guid VehicleId, IFormFile File) : IRequest;


public class UploadVehiclePhotoCommandHandler(
    ApplicationDbContext context,
    IFileProvider fileProvider) : IRequestHandler<UploadVehiclePhotoCommand>
{
    private const string Bucket = "tasandjet"; // Имя бакета MinIO

    public async Task Handle(UploadVehiclePhotoCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles
                          .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken)
                      ?? throw new NotFoundException("Транспорт не найден");

        var file = request.File;
        if (file.Length == 0)
            throw new ArgumentException("Файл пуст");

        var fileKey = $"vehicles/{vehicle.Id}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        await using var stream = file.OpenReadStream();
        var uploadRequest = new UploadFileRequest(
            Key: fileKey,
            FileName: file.FileName,
            ContentType: file.ContentType,
            Stream: stream);

        await fileProvider.UploadFileAsync(uploadRequest, cancellationToken);

        vehicle.SetPhotoUrl(fileKey);

        await context.SaveChangesAsync(cancellationToken);
    }
}
