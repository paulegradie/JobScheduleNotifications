using Server.Contracts.Client.Endpoints;

namespace Server.Contracts.Common.Request;

public abstract record RequestBase(string RouteInternal)
{
    public virtual ApiRoute GetApiRoute()
    {
        return new ApiRoute(RouteInternal);
    }

    public string ApiRoute => GetApiRoute().ToString();

    protected const string IdSegmentParam = "{id}";
    protected const string CustomerIdSegmentParam = "{customerId}";
    protected const string JobDefinitionIdSegmentParam = "{jobDefinitionId}";
}