using Api.Infrastructure.EntityFramework;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid BusinessOwnerId { get; set; }
    public virtual BusinessOwner BusinessOwner { get; set; } = null!;
    public virtual ICollection<ScheduledJob> ScheduledJobs { get; } = new List<ScheduledJob>();
} 