using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobOccurrenceIdValueGenerator : ValueGenerator<JobOccurrenceId>
{
    public override JobOccurrenceId Next(EntityEntry entry) => new JobOccurrenceId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}