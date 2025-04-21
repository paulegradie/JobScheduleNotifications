using Api.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Identity;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public sealed class ApplicationUserRecord : IdentityUser
{
    public ApplicationUserRecord()
    {
    }

    public ApplicationUserRecord(bool isAdmin, string userName) : base(userName)
    {
        IsAdmin = isAdmin;
        UserName = userName;
        Email = userName;
    }

    public bool IsAdmin { get; protected set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }

    public List<UserOrganizationRecord> UserOrganizations { get; set; } = [];

    public void MakeUserAdmin()
    {
        IsAdmin = true;
    }
}