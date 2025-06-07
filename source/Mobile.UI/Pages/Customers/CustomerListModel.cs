using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Mobile.UI.Pages.Customers.ScheduledJobs;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerListModel : ObservableObject
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

    public CustomerListModel(
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
    private async Task AddCustomerAsync()
        => await _navigation.GoToAsync(nameof(CustomerCreatePage));

    [RelayCommand]
    private async Task EditCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await _navigation.GoToAsync(
            nameof(CustomerEditPage),
            new Dictionary<string, object> { { "CustomerId", customer.Id.ToString() } });
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

    [RelayCommand]
    private async Task CreateJobAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await _navigation.GoToAsync(
            nameof(ScheduledJobCreatePage),
            new Dictionary<string, object> { { nameof(CustomerId), customer.Id.ToString() } });
    }

    [RelayCommand]
    private async Task ViewJobsAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await _navigation.GoToAsync(
            nameof(ScheduledJobListPage),
            new Dictionary<string, object> { { "CustomerId", customer.Id.ToString() } });
    }

    [RelayCommand]
    private async Task NavigateHomeAsync()
        => await _navigation.GoToAsync(nameof(LandingPage));
}