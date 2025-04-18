using System.Collections.ObjectModel;
using System.Windows.Input;
using JobScheduleNotifications.Contracts.Customers;
using JobScheduleNotifications.Mobile.Services;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class CustomerViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;
    private ObservableCollection<CustomerDto> _customers;
    private string _searchQuery = string.Empty;
    private bool _isRefreshing;
    private bool _isEditing;
    private CustomerDto? _selectedCustomer;

    public CustomerViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
        Title = "Customers";
        _customers = new ObservableCollection<CustomerDto>();
        
        LoadCustomersCommand = new Command(ExecuteLoadCustomersCommand);
        RefreshCommand = new Command(ExecuteRefreshCommand);
        AddCustomerCommand = new Command(ExecuteAddCustomerCommand);
        EditCustomerCommand = new Command<CustomerDto>(ExecuteEditCustomerCommand);
        DeleteCustomerCommand = new Command<CustomerDto>(ExecuteDeleteCustomerCommand);
        SaveCommand = new Command(ExecuteSaveCommand);
    }

#pragma warning disable CA2227
    public ObservableCollection<CustomerDto> Customers
#pragma warning restore CA2227
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                FilterCustomers();
            }
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public CustomerDto? SelectedCustomer
    {
        get => _selectedCustomer;
        set => SetProperty(ref _selectedCustomer, value);
    }

    public ICommand LoadCustomersCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddCustomerCommand { get; }
    public ICommand EditCustomerCommand { get; }
    public ICommand DeleteCustomerCommand { get; }
    public ICommand SaveCommand { get; }

    private void ExecuteLoadCustomersCommand()
    {
        LoadCustomersAsync().ConfigureAwait(false);
    }

    private void ExecuteRefreshCommand()
    {
        RefreshAsync().ConfigureAwait(false);
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

    private void ExecuteSaveCommand()
    {
        SaveCustomerAsync().ConfigureAwait(false);
    }

    private async Task LoadCustomersAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
            var customers = await _customerService.GetCustomersAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", "Failed to load customers");
            System.Diagnostics.Debug.WriteLine($"Load Customers Error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadCustomersAsync();
        IsRefreshing = false;
    }

    private async Task AddCustomerAsync()
    {
        IsEditing = true;
        SelectedCustomer = new CustomerDto();
        await _navigationService.NavigateToAsync("AddCustomerPage");
    }

    private async Task EditCustomerAsync(CustomerDto customer)
    {
        if (customer == null) return;
        
        IsEditing = true;
        SelectedCustomer = customer;
        await _navigationService.NavigateToAsync("EditCustomerPage");
    }

    private async Task DeleteCustomerAsync(CustomerDto customer)
    {
        if (customer == null) return;

        var result = await _navigationService.ShowConfirmationAsync(
            "Delete Customer",
            $"Are you sure you want to delete {customer.FirstName} {customer.LastName}?");

        if (result)
        {
            try
            {
                SetBusy(true);
                await _customerService.DeleteCustomerAsync(customer.Id);
                Customers.Remove(customer);
                await _navigationService.ShowAlertAsync("Success", "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                await _navigationService.ShowAlertAsync("Error", "Failed to delete customer");
                System.Diagnostics.Debug.WriteLine($"Delete Customer Error: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }
    }

    private async Task SaveCustomerAsync()
    {
        if (SelectedCustomer == null) return;

        try
        {
            SetBusy(true);
            if (IsEditing)
            {
                var updateDto = new UpdateCustomerDto
                {
                    FirstName = SelectedCustomer.FirstName,
                    LastName = SelectedCustomer.LastName,
                    Email = SelectedCustomer.Email,
                    PhoneNumber = SelectedCustomer.PhoneNumber,
                    Notes = SelectedCustomer.Notes
                };
                await _customerService.UpdateCustomerAsync(SelectedCustomer.Id, updateDto);
                var existingCustomer = Customers.FirstOrDefault(c => c.Id == SelectedCustomer.Id);
                if (existingCustomer != null)
                {
                    var index = Customers.IndexOf(existingCustomer);
                    Customers[index] = SelectedCustomer;
                }
            }
            else
            {
                var createDto = new CreateCustomerDto
                {
                    FirstName = SelectedCustomer.FirstName,
                    LastName = SelectedCustomer.LastName,
                    Email = SelectedCustomer.Email,
                    PhoneNumber = SelectedCustomer.PhoneNumber,
                    Notes = SelectedCustomer.Notes
                };
                var newCustomer = await _customerService.CreateCustomerAsync(createDto);
                Customers.Add(newCustomer);
            }

            IsEditing = false;
            SelectedCustomer = null;
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", "Failed to save customer");
            System.Diagnostics.Debug.WriteLine($"Save Customer Error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void FilterCustomers()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            LoadCustomersCommand.Execute(null);
            return;
        }

        var query = SearchQuery.ToLower(System.Globalization.CultureInfo.InvariantCulture);
        var filteredCustomers = Customers.Where(c =>
            c.FirstName.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
            c.LastName.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
            c.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
            c.PhoneNumber.Contains(query, StringComparison.CurrentCultureIgnoreCase)
        ).ToList();

        Customers.Clear();
        foreach (var customer in filteredCustomers)
        {
            Customers.Add(customer);
        }
    }
} 