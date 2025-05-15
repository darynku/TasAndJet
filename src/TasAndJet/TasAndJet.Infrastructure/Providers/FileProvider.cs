using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Infrastructure.Providers;

public record UploadFileRequest(string Key, string FileName, string ContentType, Stream Stream);

public class FileProvider(
    ILogger<FileProvider> logger,
    IAmazonS3 s3Client) : IFileProvider
{
    private const string Bucket = "tasandjet";
    
    public async Task UploadFileAsync(UploadFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await IsBucketExists([Bucket], cancellationToken);
            
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = Bucket,
                Key = request.Key,
                InputStream = request.Stream,
                ContentType = request.ContentType,
                Metadata = { ["file-name"] = Uri.EscapeDataString(request.FileName) },
            };
            
            await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError("Error while uploading file - {@Message}", ex.Message);
            throw new AmazonS3Exception($"Error while uploading file: {ex.Message}", ex);
        }

    }
    private async Task IsBucketExists(
        IEnumerable<string> bucketNames, CancellationToken cancellationToken = default)
    {
        HashSet<string> buckets = [..bucketNames];

        var response = await s3Client.ListBucketsAsync(cancellationToken);

        foreach (var bucketName in buckets)
        {
            var isExists = response.Buckets
                .Exists(b => b.BucketName.Equals(bucketName, StringComparison.OrdinalIgnoreCase));

            if (!isExists)
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName
                };

                await s3Client.PutBucketAsync(request, cancellationToken);
            }
        }
    }
    public string GeneratePreSignedUrl(string objectKey)
    {
        if (Uri.IsWellFormedUriString(objectKey, UriKind.Absolute))
            return objectKey;

        var config = new AmazonS3Config()
        {
            ServiceURL = $"http://192.168.31.38:9000", 
            ForcePathStyle = true,
        };

        var localClient = new AmazonS3Client("minioadmin", "minioadmin", config);
    
        var request = new GetPreSignedUrlRequest
        {
            BucketName = Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(1),
        };

        string url = localClient.GetPreSignedURL(request);
        return url.Replace("https", "http");
    }
}