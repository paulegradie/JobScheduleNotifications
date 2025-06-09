using Api.ValueTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Infrastructure.Data.TypeConverters;

public class JobCompletedPhotoIdConverter : ValueConverter<JobCompletedPhotoId, Guid>
{
    public JobCompletedPhotoIdConverter() : base(id => id.Value, guid => new JobCompletedPhotoId(guid))
    {
    }
}