using System.Collections.ObjectModel;
using System.Windows.Input;
using JobScheduleNotifications.Contracts.Customers;
using JobScheduleNotifications.Mobile.Services;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class CustomersViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;
    private ObservableCollection<CustomerDto> _customers;
    private bool _isLoading;
    private string _searchText = string.Empty;
    private CustomerDto? _selectedCustomer;

    public CustomersViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
        _customers = new ObservableCollection<CustomerDto>();
        
        LoadCustomersCommand = new Command(ExecuteLoadCustomersCommand);
        AddCustomerCommand = new Command(ExecuteAddCustomerCommand);
        EditCustomerCommand = new Command<CustomerDto>(ExecuteEditCustomerCommand);
        DeleteCustomerCommand = new Command<CustomerDto>(ExecuteDeleteCustomerCommand);
    }

#pragma warning disable CA2227
    public ObservableCollection<CustomerDto> Customers
#pragma warning restore CA2227
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterCustomers();
            }
        }
    }

    public CustomerDto? SelectedCustomer
    {
        get => _selectedCustomer;
        set => SetProperty(ref _selectedCustomer, value);
    }

    public ICommand LoadCustomersCommand { get; }
    public ICommand AddCustomerCommand { get; }
    public ICommand EditCustomerCommand { get; }
    public ICommand DeleteCustomerCommand { get; }

    private void ExecuteLoadCustomersCommand()
    {
        LoadCustomersAsync().ConfigureAwait(false);
    }

    private void ExecuteAddCustomerCommand()
    {
        AddCustomerAsync().ConfigureAwait(false);
    }

    private void ExecuteEditCustomerCommand(CustomerDto customer)
    {
        EditCustomerAsync(customer).ConfigureAwait(false);
    }

    private void ExecuteDeleteCustomerCommand(CustomerDto customer)
    {
        DeleteCustomerAsync(customer).ConfigureAwait(false);
    }

    private async Task LoadCustomersAsync()
    {
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

    private void FilterCustomers()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadCustomersAsync().ConfigureAwait(false);
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

    private async Task AddCustomerAsync()
    {
        await _navigationService.NavigateToAsync("AddCustomerPage");
    }

    private async Task EditCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        var parameters = new Dictionary<string, object> { { "CustomerId", customer.Id } };
        await _navigationService.NavigateToAsync("EditCustomerPage", parameters);
    }

    private async Task DeleteCustomerAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        
        var result = await _navigationService.ShowConfirmationAsync(
            "Delete Customer",
            $"Are you sure you want to delete {customer.FirstName} {customer.LastName}?");
            
        if (result)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(customer.Id);
                Customers.Remove(customer);
                await _navigationService.ShowAlertAsync("Success", "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                await _navigationService.ShowAlertAsync("Error", $"Failed to delete customer: {ex.Message}");
            }
        }
    }
} 