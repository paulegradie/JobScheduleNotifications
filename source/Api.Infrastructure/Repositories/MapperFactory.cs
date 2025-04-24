using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Repositories;

public class MapperFactory : IMapperFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MapperFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TOut> MapAsync<TIn, TOut>(TIn from)
    {
        var mapper = _serviceProvider.GetRequiredService<IMapFrom<TIn, TOut>>();
        return await mapper.Map(from);
    }
}