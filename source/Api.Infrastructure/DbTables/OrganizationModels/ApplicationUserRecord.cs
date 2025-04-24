using Api.Infrastructure.EntityFramework;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;

namespace Api.Infrastructure.DbTables.OrganizationModels;

[DatabaseModel]
public class ApplicationUserRecord : IdentityUser<UserId>
{
    public ApplicationUserRecord(bool isAdmin, string email)
    {
        IsAdmin = isAdmin;
        Email = email;
    }

    // “platform” admins (if you have super-user)
    public bool IsAdmin { get; protected set; }

    // our organization memberships
    // ← *all* the orgs this user belongs to
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; }
        = new List<OrganizationUser>();

    // ← *all* the customers this user (as a customer) is linked to
    public virtual ICollection<CustomerUser> CustomerUsers { get; }
        = new List<CustomerUser>();

    // refresh tokens, etc…
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiryTime { get; set; }
}