using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class BusinessOwnerIdValueConverter() : ValueConverter<CustomerId, Guid>(id => id.Value,
    guid => new CustomerId(guid),
    new ConverterMappingHints(size: 16));