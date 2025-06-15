using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            $"Delete {customer.FirstName} {customer.LastName}?");
        if (!confirm) return;

        await RunWithSpinner(async () =>
        {
            var result = await _customerRepository.DeleteCustomerAsync(customer.Id);
            if (result.IsSuccess)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Customers.Remove(customer);
                });
                await ShowSuccessAsync("Customer Deleted", $"{customer.FirstName} {customer.LastName} has been deleted successfully.");
            }
            else
            {
                await ShowErrorAsync($"Failed to delete customer: {result.ErrorMessage ?? "Unknown error"}");
            }
        }, "Failed to delete customer");
    }

    [RelayCommand]
    private async Task ViewJobsAsync(CustomerDto? customer)
    {
        if (customer == null) return;

        try
        {
            await Navigation.NavigateToScheduledJobListAsync(
                new ScheduledJobListParameters(customer.Id));
        }
        catch (ArgumentException ex)
        {
            await ShowErrorAsync(ex.Message);
        }
    }

    [RelayCommand]
    private async Task NavigateHomeAsync()
        => await Navigation.NavigateToLandingPageAsync();
}