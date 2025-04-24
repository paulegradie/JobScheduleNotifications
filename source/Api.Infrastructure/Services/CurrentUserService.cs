using Api.ValueTypes;

namespace Api.Infrastructure.Services;

/// <summary>
/// Holds information about the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    IdentityUserId? UserId { get; set; }
    CustomerId? CustomerId { get; set; }
}

public record CurrentUserService : ICurrentUserService
{
    public IdentityUserId? UserId { get; set; }
    public CustomerId? CustomerId { get; set; }
}