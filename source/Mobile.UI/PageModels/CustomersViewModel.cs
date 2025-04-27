using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.PageModels;

public partial class CustomersViewModel : ObservableObject
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

    public CustomersViewModel(
        ICustomerRepository customerRepository,
        INavigationRepository navigation)
    {
        _customerRepository = customerRepository;
        _navigation = navigation;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadCustomersAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            var result = await _customerRepository.GetCustomersAsync();
            // marshal back to main thread for any UI-bound collection ops
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Customers.Clear();
                foreach (var c in result.Value)
                    Customers.Add(c);
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ScheduleJobAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await _navigation.GoToAsync(
            nameof(AddScheduledJobPage),
            new Dictionary<string, object> { { "customerId", customer.Id.ToString() } });
    }

    [RelayCommand]
    private async Task AddCustomerAsync()
        => await _navigation.GoToAsync(nameof(CreateCustomerPage));

    [RelayCommand]
    private async Task EditCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await _navigation.GoToAsync(
            nameof(CustomerPage),
            new Dictionary<string, object> { { "customerId", customer.Id.ToString() } });
    }

    [RelayCommand]
    private async Task DeleteCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        var confirm = await _navigation.ShowConfirmationAsync(
            "Delete Customer",
            $"Delete {customer.FirstName} {customer.LastName}?"
        );
        if (!confirm) return;

        IsLoading = true;
        await _customerRepository.DeleteCustomerAsync(customer.Id);
        Customers.Remove(customer);
        IsLoading = false;
    }

    public async Task NavigateHome()
    {
        await _navigation.GoToAsync(nameof(HomePage));   
    }
}