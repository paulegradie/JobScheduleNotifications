using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobOccurrenceDomainModel : DomainModelBase<JobOccurrenceDto>
{
    public JobOccurrenceId Id { get; set; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public bool MarkedAsComplete { get; set; }
    public List<JobCompletedPhotoDomainModel> JobCompletedPhotoDomainModel { get; set; }

    public JobOccurrenceDomainStatus JobOccurrenceDomainStatus { get; set; }

    /// <summary>
    /// When this occurrence was marked complete or null if not done yet.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    public string JobDescription { get; set; }

    /// <summary>
    /// Title of the parent Scheduled Job Definition.
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    public override JobOccurrenceDto ToDto()
    {
        return new JobOccurrenceDto(
            JobOccurrenceId: Id,
            ScheduledJobDefinitionId: ScheduledJobDefinitionId,
            CustomerId: CustomerId,
            OccurrenceDate: OccurrenceDate,
            CompletedDate: CompletedDate,
            JobTitle: JobTitle,
            JobDescription: JobDescription,
            MarkedAsCompleted: MarkedAsComplete,
            JobOccurrenceDomainStatus: JobOccurrenceDomainStatus,
            JobCompletedPhotosDto: JobCompletedPhotoDomainModel.Select(x => x.ToDto()).ToList()
        );
    }

    public override void FromDto(JobOccurrenceDto dto)
    {
        Id = dto.JobOccurrenceId;
        ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
        CustomerId = dto.CustomerId;
        OccurrenceDate = dto.OccurrenceDate;
        CompletedDate = dto.CompletedDate;
        JobTitle = dto.JobTitle;
        JobDescription = dto.JobDescription;
        MarkedAsComplete = dto.MarkedAsCompleted;
        JobOccurrenceDomainStatus = dto.JobOccurrenceDomainStatus;
    }
}