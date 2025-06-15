using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Customers.ScheduledJobs;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Navigation;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerListModel : BaseViewModel
{
    private readonly ICustomerRepository _customerRepository;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

    public CustomerListModel(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadCustomersAsync()
    {
        await RunWithSpinner(async () =>
        {
            var result = await _customerRepository.GetCustomersAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Customers.Clear();
                foreach (var c in result.Value)
                    Customers.Add(c);
            });
        });
    }

    [RelayCommand]
    private async Task AddCustomerAsync()
        => await Navigation.NavigateToCustomerCreateAsync();

    [RelayCommand]
    private async Task EditCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await Navigation.NavigateToCustomerEditAsync(new CustomerParameters(customer.Id));
    }

    [RelayCommand]
    private async Task DeleteCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        var confirm = await ShowConfirmationAsync(
            "Delete Customer",
            $"Delete {customer.FirstName} {customer.LastName}?"
        );
        if (!confirm) return;

        IsLoading = true;
        await _customerRepository.DeleteCustomerAsync(customer.Id);
        Customers.Remove(customer);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task CreateJobAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await NavigateToAsync(
            nameof(ScheduledJobCreatePage),
            new Dictionary<string, object> { { nameof(CustomerId), customer.Id.ToString() } });
    }

    [RelayCommand]
    private async Task ViewJobsAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await Navigation.NavigateToCustomerJobsAsync(customer.Id);
    }

    /// <summary>
    /// Demonstration of type-safe navigation with validation
    /// </summary>
    [RelayCommand]
    private async Task ViewJobsTypeSafeAsync(CustomerDto? customer)
    {
        if (customer == null) return;

        try
        {
            // This method ensures all required parameters are provided at compile time
            // and validates them at runtime before navigation
            await Navigation.NavigateToScheduledJobListAsync(
                new ScheduledJobListParameters(customer.Id));
        }
        catch (ArgumentException ex)
        {
            // Handle validation errors gracefully
            await ShowErrorAsync(ex.Message);
        }
    }
 
    [RelayCommand]
    private async Task NavigateHomeAsync()
        => await Navigation.NavigateToLandingPageAsync();
}