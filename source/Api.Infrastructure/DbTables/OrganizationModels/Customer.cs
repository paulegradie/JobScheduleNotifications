using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class Customer
{
    public Customer()
    {
        ScheduledJobs = new List<ScheduledJob>();
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string Notes { get; set; }


    public Guid OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // ← back to all user-accounts that belong to this customer
    public virtual ICollection<CustomerUser> CustomerUsers { get; }
        = new List<CustomerUser>();


    public virtual ICollection<ScheduledJob> ScheduledJobs { get; } = new List<ScheduledJob>();

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public CustomerDto ToDto()
    {
        return new CustomerDto()
        {
            Email = Email,
            FirstName = Name,
            LastName = Name,
            Id = Id,
            PhoneNumber = PhoneNumber,
            Notes = Notes
        };
    }
}