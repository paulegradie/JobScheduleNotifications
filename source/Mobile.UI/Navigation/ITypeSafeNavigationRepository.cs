﻿﻿using Api.ValueTypes;

namespace Mobile.UI.Navigation;

/// <summary>
/// Type-safe navigation repository that ensures all required parameters are provided at compile time
/// </summary>
public interface ITypeSafeNavigationRepository
{
    // Basic navigation
    Task GoBackAsync();
    
    // Generic type-safe navigation
    Task NavigateToAsync<TPage, TParameters>(TParameters parameters) 
        where TPage : class 
        where TParameters : INavigationParameters;
    
    // Specific page navigation methods with compile-time parameter validation
    Task NavigateToCustomerListAsync();
    Task NavigateToCustomerViewAsync(CustomerParameters parameters);
    Task NavigateToCustomerEditAsync(CustomerParameters parameters);
    Task NavigateToScheduledJobListAsync(ScheduledJobListParameters parameters);
    Task NavigateToScheduledJobViewAsync(ScheduledJobParameters parameters);
    Task NavigateToScheduledJobEditAsync(ScheduledJobParameters parameters);
    Task NavigateToJobOccurrenceAsync(JobOccurrenceParameters parameters);
    Task NavigateToJobReminderAsync(JobReminderParameters parameters);
    Task NavigateToInvoiceCreateAsync(InvoiceCreateParameters parameters);
    Task NavigateToCustomerCreateAsync();
    // Landing page navigation
    Task NavigateToLandingPageAsync();
}

/// <summary>
/// Extension methods for common navigation patterns
/// </summary>
public static class TypeSafeNavigationExtensions
{
    /// <summary>
    /// Navigate to view a customer's scheduled jobs
    /// </summary>
    public static Task NavigateToCustomerJobsAsync(this ITypeSafeNavigationRepository navigation, CustomerId customerId)
    {
        return navigation.NavigateToScheduledJobListAsync(new ScheduledJobListParameters(customerId));
    }
    
    /// <summary>
    /// Navigate to view a specific job occurrence
    /// </summary>
    public static Task NavigateToJobOccurrenceAsync(this ITypeSafeNavigationRepository navigation, 
        CustomerId customerId, 
        ScheduledJobDefinitionId jobId, 
        JobOccurrenceId occurrenceId)
    {
        return navigation.NavigateToJobOccurrenceAsync(
            new JobOccurrenceParameters(customerId, jobId, occurrenceId));
    }
    
    /// <summary>
    /// Navigate to create an invoice for a job occurrence
    /// </summary>
    public static Task NavigateToCreateInvoiceAsync(this ITypeSafeNavigationRepository navigation,
        CustomerId customerId,
        ScheduledJobDefinitionId jobId,
        JobOccurrenceId occurrenceId,
        string jobDescription)
    {
        return navigation.NavigateToInvoiceCreateAsync(
            new InvoiceCreateParameters(customerId, jobId, occurrenceId));
    }
    
    /// <summary>
    /// Navigate to view a specific job reminder
    /// </summary>
    public static Task NavigateToJobReminderAsync(this ITypeSafeNavigationRepository navigation,
        CustomerId customerId,
        ScheduledJobDefinitionId jobId,
        JobOccurrenceId occurrenceId,
        JobReminderId reminderId)
    {
        return navigation.NavigateToJobReminderAsync(
            new JobReminderParameters(customerId, jobId, occurrenceId, reminderId));
    }
}
