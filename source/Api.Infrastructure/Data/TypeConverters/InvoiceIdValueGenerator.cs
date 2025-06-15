using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class InvoiceIdValueGenerator : ValueGenerator<InvoiceId>
{
    public override InvoiceId Next(EntityEntry entry) => new InvoiceId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}
