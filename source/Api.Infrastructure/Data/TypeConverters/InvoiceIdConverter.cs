using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class InvoiceIdConverter : ValueConverter<InvoiceId, Guid>
{
    public InvoiceIdConverter() : base(id => id.Value, guid => new InvoiceId(guid))
    {
    }
}
