using Api.Infrastructure.Identity;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class OrganizationUser
{
    public IdentityUserId IdentityUserId { get; set; }
    public virtual ApplicationUserRecord User { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public OrganizationRole Role { get; set; }
}