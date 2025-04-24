namespace Api.Infrastructure.Repositories;

public interface IMapFrom<in TIn, TOut>
{
    Task<TOut> Map(TIn from);
}