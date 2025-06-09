using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed record GetDashboardContentRequest() : RequestBase(Route)
{
    public const string Route = "api/dashboard";
}