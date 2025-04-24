namespace Api.Infrastructure.Repositories;

public interface IMapperFactory
{
    Task<TOut> MapAsync<TIn, TOut>(TIn from);
}