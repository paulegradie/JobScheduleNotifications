namespace Api.Infrastructure.DbTables.OrganizationModels;

public class Organization
{
    public Guid Id     { get; set; }
    public string Name { get; set; } = "";

    // ← back to all user-members of this org
    public virtual ICollection<OrganizationUser> OrganizationUsers { get; }
        = new List<OrganizationUser>();

    // ← the business’s customers
    public virtual ICollection<Customer> Customers { get; }
        = new List<Customer>();
}