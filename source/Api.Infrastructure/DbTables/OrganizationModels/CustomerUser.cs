using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class CustomerUser
{
    public UserId UserId           { get; set; }
    public virtual ApplicationUserRecord User { get; set; } = null!;

    public Guid CustomerId         { get; set; }
    public virtual Customer Customer { get; set; } = null!;
}