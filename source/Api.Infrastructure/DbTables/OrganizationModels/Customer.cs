using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class Customer
{
    public CustomerId CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string Notes { get; set; }


    public OrganizationId OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // ← back to all user-accounts that belong to this customer
    public virtual ICollection<CustomerUser> CustomerUsers { get; } = new List<CustomerUser>();


    public virtual ICollection<ScheduledJobDefinition> ScheduledJobDefinitions { get; } = new List<ScheduledJobDefinition>();


    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public Customer DefineNewJob(
        ScheduledJobDefinitionId id,
        string cronExpression,
        string title,
        string description,
        DateTime anchorDate)
    {
        ScheduledJobDefinitions.Add(new ScheduledJobDefinition
        {
            ScheduledJobDefinitionId = id,
            CustomerId = CustomerId,
            CronExpression = cronExpression,
            AnchorDate = anchorDate,
            Title = title,
            Description = description
        });
        return this;
    }

    public CustomerDto ToDto()
    {
        return new CustomerDto()
        {
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Id = CustomerId,
            PhoneNumber = PhoneNumber,
            Notes = Notes
        };
    }

    public Customer UpdateFirstName(string? firstName)
    {
        if (firstName is not null)
        {
            FirstName = firstName;
        }

        return this;
    }

    public Customer UpdateLastName(string? lastName)
    {
        if (lastName is not null)
        {
            LastName = lastName;
        }

        return this;
    }
}