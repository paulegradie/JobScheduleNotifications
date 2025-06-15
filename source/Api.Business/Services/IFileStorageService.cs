namespace Api.Business.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to the configured storage location
    /// </summary>
    /// <param name="stream">The file stream to save</param>
    /// <param name="fileName">The desired file name</param>
    /// <param name="contentType">The MIME content type of the file</param>
    /// <returns>The storage path/URL where the file was saved</returns>
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType);
    
    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="filePath">The path/URL of the file to delete</param>
    Task DeleteFileAsync(string filePath);
    
    /// <summary>
    /// Gets a URL for accessing the file (may be temporary for cloud storage)
    /// </summary>
    /// <param name="filePath">The storage path of the file</param>
    /// <returns>A URL that can be used to access the file</returns>
    Task<string> GetFileUrlAsync(string filePath);
    
    /// <summary>
    /// Gets the storage location type for this service
    /// </summary>
    FileStorageLocation StorageLocation { get; }
}

public enum FileStorageLocation
{
    Local = 0,
    AwsS3 = 1
}
