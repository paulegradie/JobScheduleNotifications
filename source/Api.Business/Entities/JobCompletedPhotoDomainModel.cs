using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobCompletedPhotoDomainModel() : DomainModelBase<JobCompletedPhotoDto>
{
    public JobCompletedPhotoId JobCompletedPhotoId { get; set; }
    public string PhotoUri { get; set; } // local for now
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public CustomerId CustomerId { get; set; }

    public override JobCompletedPhotoDto ToDto()
    {
        return new JobCompletedPhotoDto(JobOccurrenceId, CustomerId, JobCompletedPhotoId, PhotoUri);
    }

    public override void FromDto(JobCompletedPhotoDto dto)
    {
        throw new NotImplementedException();
    }
}