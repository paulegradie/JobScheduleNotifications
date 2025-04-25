using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class RecurrencePatternIdValueGenerator : ValueGenerator<RecurrencePatternId>
{
    public override RecurrencePatternId Next(EntityEntry entry) => new RecurrencePatternId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}