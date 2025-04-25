using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobOccurrenceIdConverter : ValueConverter<JobOccurrenceId, Guid>
{
    public JobOccurrenceIdConverter() : base(id => id.Value, guid => new JobOccurrenceId(guid))
    {
    }
}