using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.EntityFramework.EntityConventions;

public class AutoIncludeConvention : IEntityPropertyConvention
{
    public void Apply(ModelBuilder modelBuilder, EntityTypeBuilder entityTypeBuilder, PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute<AutoIncludeAttribute>() is null) return;
        modelBuilder.Entity(propertyInfo.PropertyType).Navigation(propertyInfo.Name).AutoInclude();
    }
}