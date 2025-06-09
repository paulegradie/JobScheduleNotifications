namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed class JobCompletedPhotoDeletedDto
{
    public JobCompletedPhotoDeletedDto(bool deleted)
    {
        Deleted = deleted;
    }

    public bool Deleted { get; set; }
}