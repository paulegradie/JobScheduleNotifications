namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed class JobPhotoUploadDto
{
    public bool Success { get; init; }
    public string FilePath { get; init; }
    public long FileSize { get; init; }

    public JobPhotoUploadDto(bool success, string filePath, long fileSize)
    {
        Success = success;
        FilePath = filePath;
        FileSize = fileSize;
    }
}