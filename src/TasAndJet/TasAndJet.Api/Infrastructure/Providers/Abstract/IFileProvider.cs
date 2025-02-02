namespace TasAndJet.Api.Infrastructure.Providers.Abstract;

public interface IFileProvider
{
    Task UploadFileAsync(UploadFileRequest request, CancellationToken cancellationToken);
}