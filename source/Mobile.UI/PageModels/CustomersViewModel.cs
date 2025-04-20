using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Services;
using Server.Contracts.Customers;
using ICustomerService = Mobile.Core.Interfaces.ICustomerService;

namespace Mobile.UI.PageModels;

public partial class CustomersViewModel : ObservableObject
{
    public const string Title = "OMG TODO";
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private CustomerDto? _selectedCustomer;

    public bool IsNotLoading => !IsLoading;

    [ObservableProperty]
    private ObservableCollection<CustomerDto> _customers = new();

    public CustomersViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadCustomers()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            var customers = await _customerService.GetCustomersAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", $"Failed to load customers: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddCustomer()
    {
        await _navigationService.NavigateToAsync("AddCustomerPage");
    }

    [RelayCommand]
    private async Task EditCustomer(CustomerDto? customer)
    {
        if (customer == null) return;
        var parameters = new Dictionary<string, object> { { "CustomerId", customer.Id } };
        await _navigationService.NavigateToAsync("EditCustomerPage", parameters);
    }

    [RelayCommand]
    private async Task DeleteCustomer(CustomerDto? customer)
    {
        if (customer == null) return;
        
        var result = await _navigationService.ShowConfirmationAsync(
            "Delete Customer",
            $"Are you sure you want to delete {customer.FirstName} {customer.LastName}?");
            
        if (result)
        {
            try
            {
                IsLoading = true;
                await _customerService.DeleteCustomerAsync(customer.Id);
                Customers.Remove(customer);
                await _navigationService.ShowAlertAsync("Success", "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                await _navigationService.ShowAlertAsync("Error", $"Failed to delete customer: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterCustomers();
    }

    [RelayCommand]
    public void Refresh()
    {
        Console.WriteLine("Refreshed!");
    }

    private void FilterCustomers()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadCustomersCommand.Execute(null);
            return;
        }

        var filteredCustomers = Customers.Where(c =>
            c.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            c.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            c.PhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Customers.Clear();
        foreach (var customer in filteredCustomers)
        {
            Customers.Add(customer);
        }
    }
}