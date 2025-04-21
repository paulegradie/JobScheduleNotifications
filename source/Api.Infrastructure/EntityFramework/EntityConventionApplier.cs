namespace Api.Infrastructure.EntityFramework;

public class EntityConventionApplier :IEntityConventionApplier
{
    private readonly IEnumerable<IEntityPropertyConvention> conventions;

    // autofac returns an array of registrations when multiple are registered under the same name
    public EntityConventionApplier(IEnumerable<IEntityPropertyConvention> conventions)
    {
        this.conventions = conventions;
    }
    
    public void Apply()
    {
        throw new NotImplementedException();
    }
}