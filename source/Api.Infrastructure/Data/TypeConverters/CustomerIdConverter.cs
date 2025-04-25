using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class CustomerIdConverter : ValueConverter<CustomerId, Guid>
{
    public CustomerIdConverter() : base(id => id.Value, guid => new CustomerId(guid))
    {
    }
}