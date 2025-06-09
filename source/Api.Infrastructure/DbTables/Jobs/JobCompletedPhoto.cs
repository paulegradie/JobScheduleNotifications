using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobCompletedPhoto
{
    public JobCompletedPhotoId JobCompletedPhotoId { get; set; }
    public CustomerId CustomerId { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
    public virtual JobOccurrence JobOccurrence { get; set; } = null!;
}