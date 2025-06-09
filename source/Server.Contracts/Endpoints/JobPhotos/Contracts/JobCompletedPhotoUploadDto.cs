using Api.ValueTypes;

namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed class JobCompletedPhotoUploadDto
{
    public bool Success { get; init; }
    public string FilePath { get; init; }
    public long FileSize { get; init; }
    public JobCompletedPhotoId JobCompletedPhotoId { get; set; }

    public JobCompletedPhotoUploadDto(bool success, string filePath, long fileSize)
    {
        Success = success;
        FilePath = filePath;
        FileSize = fileSize;
    }
}