namespace TasAndJet.Contracts.Requests;

public record UploadFileRequest(
    string FileName, 
    string ContentType,
    long FileSize);