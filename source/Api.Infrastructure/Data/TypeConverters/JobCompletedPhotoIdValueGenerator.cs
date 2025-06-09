using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobCompletedPhotoIdValueGenerator : ValueGenerator<JobCompletedPhotoId>
{
    public override JobCompletedPhotoId Next(EntityEntry entry) => new JobCompletedPhotoId(Guid.NewGuid());

    public override bool GeneratesTemporaryValues => false;
}