using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class OrganizationIdValueGenerator : ValueGenerator<OrganizationId>
{
    public override OrganizationId Next(EntityEntry entry)
        => new OrganizationId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}