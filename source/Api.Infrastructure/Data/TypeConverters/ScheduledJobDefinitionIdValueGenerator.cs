using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class ScheduledJobDefinitionIdValueGenerator : ValueGenerator<ScheduledJobDefinitionId>
{
    public override ScheduledJobDefinitionId Next(EntityEntry entry) => new ScheduledJobDefinitionId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}