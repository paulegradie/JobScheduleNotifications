using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class UserIdValueConverter() : ValueConverter<IdentityUserId, Guid>(id => id.Value,
    guid => new IdentityUserId(guid),
    new ConverterMappingHints(size: 16));