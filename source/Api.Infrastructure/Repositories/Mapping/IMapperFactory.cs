namespace Api.Infrastructure.Repositories.Mapping;

public interface IMapperFactory
{
    Task<TOut> MapAsync<TIn, TOut>(TIn from);
}