using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class CustomerUser
{
    public IdentityUserId IdentityUserId { get; set; }
    public virtual ApplicationUserRecord User { get; set; } = null!;

    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
}