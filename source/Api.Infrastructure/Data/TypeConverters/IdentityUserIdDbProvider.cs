using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class IdentityUserIdValueConverter() : ValueConverter<IdentityUserId, Guid>(id => id.Value,
    guid => new IdentityUserId(guid),
    new ConverterMappingHints(size: 16));

public class IdentityUserIdValueGenerator : ValueGenerator<IdentityUserId>
{
    public override IdentityUserId Next(EntityEntry entry)
        => new IdentityUserId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}