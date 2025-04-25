using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class ScheduledJobDefinitionIdConverter : ValueConverter<ScheduledJobDefinitionId, Guid>
{
    public ScheduledJobDefinitionIdConverter() : base(id => id.Value, guid => new ScheduledJobDefinitionId(guid))
    {
    }
}