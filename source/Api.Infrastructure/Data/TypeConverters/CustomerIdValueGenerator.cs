using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class CustomerIdValueGenerator : ValueGenerator<CustomerId>
{
    public override CustomerId Next(EntityEntry entry) => new CustomerId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}