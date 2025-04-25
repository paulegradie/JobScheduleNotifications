using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobReminderIdConverter : ValueConverter<JobReminderId, Guid>
{
    public JobReminderIdConverter() : base(id => id.Value, guid => new JobReminderId(guid))
    {
    }
}