﻿using Api.Business.Services;
using Microsoft.AspNetCore.Hosting;

namespace Api.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _baseDirectory;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _baseDirectory = Path.Combine(_environment.ContentRootPath, "UploadedInvoices");
        Directory.CreateDirectory(_baseDirectory);
    }

    public FileStorageLocation StorageLocation => FileStorageLocation.Local;

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string contentType)
    {
        // Generate unique filename to avoid conflicts
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_baseDirectory, uniqueFileName);
        
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        
        return filePath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public Task<string> GetFileUrlAsync(string filePath)
    {
        // For local files, return the file path as-is
        // In a real application, you might want to return a URL that serves the file
        return Task.FromResult(filePath);
    }
}
