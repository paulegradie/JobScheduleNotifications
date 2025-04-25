namespace Api.Infrastructure.Repositories.Mapping;

public interface IMapFrom<in TIn, TOut>
{
    Task<TOut> Map(TIn from);
}