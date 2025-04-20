using Server.Contracts.Client.Endpoints.Home;

namespace Server.Contracts.Client.Request;

public abstract record RequestBase(string Route, string? Filter = null)
{
    public virtual ApiRoute GetActionRoute()
    {
        var route = new ApiRoute(Route);
        if (Filter != null)
            route.AddQueryParam("filter", Filter);

        return route;
    };
    
    
}