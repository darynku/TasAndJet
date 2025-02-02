namespace TasAndJet.Api.Contracts.Data.File;

public class FileData
{
    public Guid Id { get; init; }
    public required string Path { get; init; }
    public required long FileSize { get; init; }
    public required string ContentType { get; init; }
    public required DateTime UploadDate { get; init; }
}