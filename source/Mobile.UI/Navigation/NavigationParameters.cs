using Api.ValueTypes;

namespace Mobile.UI.Navigation;

/// <summary>
/// Base interface for all navigation parameters to ensure type safety
/// </summary>
public interface INavigationParameters
{
    /// <summary>
    /// Convert the parameters to a dictionary for Shell navigation
    /// </summary>
    Dictionary<string, object> ToDictionary();
    
    /// <summary>
    /// Validate that all required parameters are present and valid
    /// </summary>
    void Validate();
}

/// <summary>
/// Navigation parameters for CustomerListPage
/// </summary>
public record CustomerListParameters() : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new();
    public void Validate() { }
}

/// <summary>
/// Navigation parameters for CustomerViewPage and CustomerEditPage
/// </summary>
public record CustomerParameters(CustomerId CustomerId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString()
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
    }
}

/// <summary>
/// Navigation parameters for ScheduledJobListPage
/// </summary>
public record ScheduledJobListParameters(CustomerId CustomerId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString()
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
    }
}

/// <summary>
/// Navigation parameters for ScheduledJobViewPage and ScheduledJobEditPage
/// </summary>
public record ScheduledJobParameters(
    CustomerId CustomerId, 
    ScheduledJobDefinitionId ScheduledJobDefinitionId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString(),
        ["ScheduledJobDefinitionId"] = ScheduledJobDefinitionId.Value.ToString()
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
        if (ScheduledJobDefinitionId.Value == Guid.Empty)
            throw new ArgumentException("ScheduledJobDefinitionId cannot be empty", nameof(ScheduledJobDefinitionId));
    }
}

/// <summary>
/// Navigation parameters for ViewJobOccurrencePage
/// </summary>
public record JobOccurrenceParameters(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString(),
        ["ScheduledJobDefinitionId"] = ScheduledJobDefinitionId.Value.ToString(),
        ["JobOccurrenceId"] = JobOccurrenceId.Value.ToString()
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
        if (ScheduledJobDefinitionId.Value == Guid.Empty)
            throw new ArgumentException("ScheduledJobDefinitionId cannot be empty", nameof(ScheduledJobDefinitionId));
        if (JobOccurrenceId.Value == Guid.Empty)
            throw new ArgumentException("JobOccurrenceId cannot be empty", nameof(JobOccurrenceId));
    }
}

/// <summary>
/// Navigation parameters for JobReminderPage
/// </summary>
public record JobReminderParameters(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    JobReminderId JobReminderId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString(),
        ["ScheduledJobDefinitionId"] = ScheduledJobDefinitionId.Value.ToString(),
        ["JobOccurrenceId"] = JobOccurrenceId.Value.ToString(),
        ["JobReminderId"] = JobReminderId.Value.ToString()
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
        if (ScheduledJobDefinitionId.Value == Guid.Empty)
            throw new ArgumentException("ScheduledJobDefinitionId cannot be empty", nameof(ScheduledJobDefinitionId));
        if (JobOccurrenceId.Value == Guid.Empty)
            throw new ArgumentException("JobOccurrenceId cannot be empty", nameof(JobOccurrenceId));
        if (JobReminderId.Value == Guid.Empty)
            throw new ArgumentException("JobReminderId cannot be empty", nameof(JobReminderId));
    }
}

/// <summary>
/// Navigation parameters for InvoiceCreatePage
/// </summary>
public record InvoiceCreateParameters(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["CustomerId"] = CustomerId.Value.ToString(),
        ["ScheduledJobDefinitionId"] = ScheduledJobDefinitionId.Value.ToString(),
        ["JobOccurrenceId"] = JobOccurrenceId.Value.ToString(),
    };
    
    public void Validate()
    {
        if (CustomerId.Value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(CustomerId));
        if (ScheduledJobDefinitionId.Value == Guid.Empty)
            throw new ArgumentException("ScheduledJobDefinitionId cannot be empty", nameof(ScheduledJobDefinitionId));
        if (JobOccurrenceId.Value == Guid.Empty)
            throw new ArgumentException("JobOccurrenceId cannot be empty", nameof(JobOccurrenceId));
        // if (string.IsNullOrWhiteSpace(JobDescription))
        //     throw new ArgumentException("JobDescription cannot be empty", nameof(JobDescription));
    }
}
