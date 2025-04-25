using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobReminderIdValueGenerator : ValueGenerator<JobReminderId>
{
    public override JobReminderId Next(EntityEntry entry) => new JobReminderId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}