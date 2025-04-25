using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Identity;

/// <summary>
/// Static container for all identity role names.
/// </summary>
public static class Roles
{
    public const string OrganizationOwner = "Owner";
    public const string OrganizationAdmin = "Admin";
    public const string OrganizationMember = "Member";
    public const string Customer = "Customer";

    /// <summary>
    /// All roles defined in the system.
    /// </summary>
    public static IReadOnlyList<string> All
        => [OrganizationOwner, OrganizationAdmin, OrganizationMember, Customer];
}

/// <summary>
/// Strongly-typed wrapper for organization roles.
/// </summary>
public sealed record OrganizationRole(string Name)
{
    public static OrganizationRole Owner => new(Roles.OrganizationOwner);
    public static OrganizationRole Admin => new(Roles.OrganizationAdmin);
    public static OrganizationRole Member => new(Roles.OrganizationMember);

    public override string ToString() => Name;
    public static implicit operator string(OrganizationRole role) => role.Name;
}

/// <summary>
/// Strongly-typed wrapper for the customer role.
/// </summary>
public sealed record CustomerRole(string Name)
{
    public static CustomerRole Customer => new(Roles.Customer);

    public override string ToString() => Name;
    public static implicit operator string(CustomerRole role) => role.Name;
}

/// <summary>
/// Names for authorization policies, each tied to specific role-based capabilities.
/// </summary>
public static class PolicyNames
{
    // Basic role checks
    public const string CustomerPolicy = "CustomerPolicy"; // Only Customer role
    public const string MemberPolicy = "MemberPolicy"; // Only OrganizationMember
    public const string AdminPolicy = "AdminPolicy"; // Only OrganizationAdmin
    public const string OwnerPolicy = "OwnerPolicy"; // Only OrganizationOwner

    // Shared capabilities
    public const string ManageOwnCustomers = "ManageOwnCustomers"; // Customer, Member, Admin, Owner
    public const string ManageMembers = "ManageMembers"; // Admin, Owner
    public const string ManageAdmins = "ManageAdmins"; // Owner only
    public const string ManageOrganizationSettings = "ManageOrganizationSettings"; // Owner only
}

/// <summary>
/// Extension methods to register role-based authorization policies.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds policies that map directly to individual roles or capability sets.
    /// </summary>
    public static IServiceCollection AddRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // ----- Basic role checks -----
            options.AddPolicy(PolicyNames.CustomerPolicy, policy =>
                policy.RequireRole(Roles.Customer));

            options.AddPolicy(PolicyNames.MemberPolicy, policy =>
                policy.RequireRole(Roles.OrganizationMember));

            options.AddPolicy(PolicyNames.AdminPolicy, policy =>
                policy.RequireRole(Roles.OrganizationAdmin));

            options.AddPolicy(PolicyNames.OwnerPolicy, policy =>
                policy.RequireRole(Roles.OrganizationOwner));

            // ----- Capability-based policies -----
            // Any authenticated user (including customers) with domain-specific link can manage their own customers
            options.AddPolicy(PolicyNames.ManageOwnCustomers, policy =>
                policy.RequireRole(
                    Roles.Customer,
                    Roles.OrganizationMember,
                    Roles.OrganizationAdmin,
                    Roles.OrganizationOwner));

            // Members of an organization that are Admin or Owner can add/remove members
            options.AddPolicy(PolicyNames.ManageMembers, policy =>
                policy.RequireRole(
                    Roles.OrganizationAdmin,
                    Roles.OrganizationOwner));

            // Only the OrganizationOwner can add/remove admins
            options.AddPolicy(PolicyNames.ManageAdmins, policy =>
                policy.RequireRole(Roles.OrganizationOwner));

            // Only the OrganizationOwner can change organization-wide settings
            options.AddPolicy(PolicyNames.ManageOrganizationSettings, policy =>
                policy.RequireRole(Roles.OrganizationOwner));
        });

        return services;
    }
}