﻿﻿﻿using Amazon.S3;
using Amazon.S3.Model;
using Api.Business.Services;
using Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Api.Infrastructure.Services;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageSettings _config;

    public S3FileStorageService(IAmazonS3 s3Client, IOptions<StorageConfiguration> config)
    {
        _s3Client = s3Client;
        _config = config.Value.S3;
    }

    public FileStorageLocation StorageLocation => FileStorageLocation.AwsS3;

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string contentType)
    {
        var key = $"invoices/{Guid.NewGuid()}_{fileName}";
        
        var request = new PutObjectRequest
        {
            BucketName = _config.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };

        await _s3Client.PutObjectAsync(request);
        
        return key; // Return the S3 key as the file path
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _config.BucketName,
            Key = filePath
        };

        await _s3Client.DeleteObjectAsync(request);
    }

    public async Task<string> GetFileUrlAsync(string filePath)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _config.BucketName,
            Key = filePath,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(1) // URL expires in 1 hour
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }
}


