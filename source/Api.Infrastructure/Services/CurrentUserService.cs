using Api.ValueTypes;

namespace Api.Infrastructure.Services;

/// <summary>
/// Holds information about the current authenticated user.
/// </summary>
public interface ICurrentUserContext
{
    IdentityUserId? UserId { get; set; }
    CustomerId? CustomerId { get; set; }
    OrganizationId? OrganizationId { get; set; }
}

public record CurrentUserContext : ICurrentUserContext
{
    public IdentityUserId? UserId { get; set; }
    public CustomerId? CustomerId { get; set; }
    public OrganizationId? OrganizationId { get; set; }
}