using Server.Contracts.Client.Endpoints;

namespace Server.Contracts.Client.Request;

public abstract record RequestBase(string RouteInternal)
{
    public virtual ApiRoute GetApiRoute()
    {
        return new ApiRoute(RouteInternal);
    }

    public string ApiRoute => GetApiRoute().ToString();
}