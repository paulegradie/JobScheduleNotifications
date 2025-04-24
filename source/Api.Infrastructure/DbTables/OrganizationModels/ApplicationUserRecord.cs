using Api.Infrastructure.EntityFramework;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;

namespace Api.Infrastructure.DbTables.OrganizationModels;

[DatabaseModel]
public class ApplicationUserRecord : IdentityUser<IdentityUserId>
{
    // Create a new user with a generated Id and initialized username/email
    public ApplicationUserRecord(string email)
    {
        Id = IdentityUserId.New();
        UserName = email;
        NormalizedUserName = email.ToUpperInvariant();
        Email = email;
        NormalizedEmail = email.ToUpperInvariant();
    }

    /// <summary>
    /// All the organizations this user belongs to.
    /// </summary>
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; private set; }
        = new List<OrganizationUser>();

    /// <summary>
    /// All the customers this user (as a customer) is linked to.
    /// </summary>
    public virtual ICollection<CustomerUser> CustomerUsers { get; private set; }
        = new List<CustomerUser>();

    /// <summary>
    /// Used for refresh-token flow; nullable until first sign-in.
    /// </summary>
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Optional: if you want a "current" customer on the user entity
    // public Guid? CurrentCustomerId { get; set; }
}