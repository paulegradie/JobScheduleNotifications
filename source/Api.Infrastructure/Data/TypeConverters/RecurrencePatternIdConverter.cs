using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class RecurrencePatternIdConverter : ValueConverter<RecurrencePatternId, Guid>
{
    public RecurrencePatternIdConverter() : base(id => id.Value, guid => new RecurrencePatternId(guid))
    {
    }
}