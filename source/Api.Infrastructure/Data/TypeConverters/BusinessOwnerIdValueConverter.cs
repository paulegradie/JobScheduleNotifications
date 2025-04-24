using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class BusinessOwnerIdValueConverter() : ValueConverter<BusinessOwnerId, Guid>(id => id.Value,
    guid => new BusinessOwnerId(guid),
    new ConverterMappingHints(size: 16));