using Server.Contracts.Client.Endpoints.Home;

namespace Server.Contracts.Client.Request;

public abstract record RequestBase(string RouteInternal, string? Filter = null)
{
    public virtual ApiRoute GetApiRoute()
    {
        var route = new ApiRoute(RouteInternal);
        if (Filter != null)
            route.AddQueryParam("filter", Filter);

        return route;
    }

    public string ApiRoute => GetApiRoute().ToString();

}