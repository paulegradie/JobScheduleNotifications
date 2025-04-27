using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Auth.Contracts;

public sealed record RegisterNewAdminRequest() : RequestBase(Route)
{
    public const string Route = "/api/user/register";

    public string Email { get; set; }
    public string Password { get; set; }
    public string BusinessName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string BusinessDescription { get; set; }
}