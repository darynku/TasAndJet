﻿namespace TasAndJet.Infrastructure.Options;

public class MinioOptions 
{
    public const string MinioSection = "MinioOptions";

    public string Endpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public bool WithSsl { get; init; } = false;

}