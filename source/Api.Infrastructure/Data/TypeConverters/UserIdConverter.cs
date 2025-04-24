using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class UserIdValueConverter() : ValueConverter<UserId, Guid>(id => id.Value,
    guid => new UserId(guid),
    new ConverterMappingHints(size: 16));