namespace TasAndJet.Infrastructure.Providers.Abstract;

public interface IFileProvider
{
    Task UploadFileAsync(UploadFileRequest request, CancellationToken cancellationToken);
    string GeneratePreSignedUrl(string objectKey);
}