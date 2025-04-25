using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class OrganizationIdValueConverter() : ValueConverter<OrganizationId, Guid>(id => id.Value,
    guid => new OrganizationId(guid),
    new ConverterMappingHints(size: 16));