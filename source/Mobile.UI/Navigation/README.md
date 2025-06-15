# Type-Safe Navigation System

## Overview

This navigation system provides compile-time safety for page navigation parameters, eliminating runtime errors caused by missing or incorrect navigation parameters.

## Problem Solved

**Before (Error-Prone):**
```csharp
// Easy to miss parameters or make typos
await _navigation.GoToAsync("JobReminderPage", new Dictionary<string, object> 
{
    ["CustomerId"] = customerId.ToString(),
    ["ScheduledJobDefinitionId"] = jobId.ToString()
    // Missing JobOccurrenceId and JobReminderId - runtime error!
});
```

**After (Type-Safe):**
```csharp
// Compiler ensures all required parameters are provided
await _navigation.NavigateToJobReminderAsync(customerId, jobId, occurrenceId, reminderId);
```

## Key Benefits

1. **Compile-Time Safety**: Missing parameters are caught at compile time, not runtime
2. **IntelliSense Support**: Full IDE support with parameter hints and validation
3. **Refactoring Safety**: Renaming parameters updates all usages automatically
4. **Self-Documenting**: Method signatures clearly show what parameters are required
5. **Validation**: Built-in parameter validation with meaningful error messages
6. **Backward Compatible**: Works alongside existing navigation code

## Usage Examples

### Basic Navigation

```csharp
public class MyViewModel
{
    private readonly ITypeSafeNavigationRepository _navigation;
    
    public MyViewModel(ITypeSafeNavigationRepository navigation)
    {
        _navigation = navigation;
    }
    
    // Navigate to customer list
    await _navigation.NavigateToCustomerListAsync();
    
    // Navigate to view a customer
    await _navigation.NavigateToCustomerViewAsync(new CustomerParameters(customerId));
    
    // Navigate to customer's jobs (using extension method)
    await _navigation.NavigateToCustomerJobsAsync(customerId);
}
```

### Advanced Navigation

```csharp
// Navigate to job occurrence with all required parameters
await _navigation.NavigateToJobOccurrenceAsync(customerId, jobId, occurrenceId);

// Navigate to create invoice
await _navigation.NavigateToCreateInvoiceAsync(customerId, jobId, occurrenceId, jobDescription);

// Navigate to specific reminder (now with all required parameters!)
await _navigation.NavigateToJobReminderAsync(customerId, jobId, occurrenceId, reminderId);
```

### Generic Navigation

```csharp
// For dynamic scenarios where page type is determined at runtime
await _navigation.NavigateToAsync<CustomerViewPage, CustomerParameters>(
    new CustomerParameters(customerId));
```

## Parameter Classes

All navigation parameters implement `INavigationParameters` and provide:

- **Validation**: `Validate()` method ensures all parameters are valid
- **Dictionary Conversion**: `ToDictionary()` converts to Shell navigation format
- **Type Safety**: Strongly-typed properties prevent parameter errors

### Available Parameter Classes

- `CustomerParameters` - For customer-related pages
- `ScheduledJobParameters` - For job-related pages  
- `JobOccurrenceParameters` - For job occurrence pages
- `JobReminderParameters` - For reminder pages (with all required parameters!)
- `InvoiceCreateParameters` - For invoice creation

## Migration Guide

### Step 1: Inject the New Service

```csharp
// Old
private readonly INavigationRepository _navigation;

// New (add alongside existing)
private readonly ITypeSafeNavigationRepository _typeSafeNavigation;

public MyViewModel(
    INavigationRepository navigation,
    ITypeSafeNavigationRepository typeSafeNavigation)
{
    _navigation = navigation;
    _typeSafeNavigation = typeSafeNavigation;
}
```

### Step 2: Replace Navigation Calls

```csharp
// Old
await _navigation.GoToAsync("CustomerViewPage", new Dictionary<string, object> 
{
    ["CustomerId"] = customerId.ToString()
});

// New
await _typeSafeNavigation.NavigateToCustomerViewAsync(new CustomerParameters(customerId));

// Or even simpler with extension methods
await _typeSafeNavigation.NavigateToCustomerViewAsync(customerId);
```

### Step 3: Handle Validation

```csharp
try
{
    await _typeSafeNavigation.NavigateToCustomerViewAsync(parameters);
}
catch (ArgumentException ex)
{
    await _typeSafeNavigation.ShowAlertAsync("Navigation Error", ex.Message);
}
```

## Extension Methods

The system includes convenient extension methods for common navigation patterns:

```csharp
// Navigate to customer's jobs
await _navigation.NavigateToCustomerJobsAsync(customerId);

// Navigate to job occurrence  
await _navigation.NavigateToJobOccurrenceAsync(customerId, jobId, occurrenceId);

// Navigate to create invoice
await _navigation.NavigateToCreateInvoiceAsync(customerId, jobId, occurrenceId, description);

// Navigate to job reminder
await _navigation.NavigateToJobReminderAsync(customerId, jobId, occurrenceId, reminderId);
```

## Adding New Pages

To add navigation support for a new page:

1. **Create Parameter Class**:
```csharp
public record MyPageParameters(string RequiredParam, int OptionalParam = 0) : INavigationParameters
{
    public Dictionary<string, object> ToDictionary() => new()
    {
        ["RequiredParam"] = RequiredParam,
        ["OptionalParam"] = OptionalParam.ToString()
    };
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(RequiredParam))
            throw new ArgumentException("RequiredParam cannot be empty");
    }
}
```

2. **Add to Interface**:
```csharp
Task NavigateToMyPageAsync(MyPageParameters parameters);
```

3. **Add to Implementation**:
```csharp
public Task NavigateToMyPageAsync(MyPageParameters parameters) => 
    NavigateToAsync<MyPage, MyPageParameters>(parameters);
```

4. **Register Page Name**:
```csharp
{ typeof(MyPage), nameof(MyPage) }
```

## Best Practices

1. **Use Extension Methods**: They provide the cleanest syntax
2. **Handle Validation**: Always wrap navigation in try-catch for validation errors
3. **Prefer Type-Safe**: Use the new system for all new code
4. **Gradual Migration**: Migrate existing code incrementally
5. **Document Parameters**: Use XML comments to document parameter requirements

## Troubleshooting

### Common Issues

1. **Page Not Registered**: Add page type to `PageNames` dictionary
2. **Validation Errors**: Check parameter validation logic
3. **Missing Parameters**: Compiler will catch these at build time
4. **DI Registration**: Ensure `ITypeSafeNavigationRepository` is registered

### Error Messages

- `"Page type X is not registered for navigation"` - Add to PageNames dictionary
- `"CustomerId cannot be empty"` - Parameter validation failed
- `"Cannot resolve ITypeSafeNavigationRepository"` - Check DI registration
