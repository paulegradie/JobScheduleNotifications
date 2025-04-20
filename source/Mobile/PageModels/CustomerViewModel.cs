using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JobScheduleNotifications.Contracts.Customers;

namespace Mobile.PageModels;

public partial class CustomerViewModel : ObservableValidator
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<CustomerDto> customers = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    private bool isBusy;

    [ObservableProperty]
    private string title = "Customers";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCustomerCommand))]
    private CustomerDto? selectedCustomer;

    [ObservableProperty]
    private string? validationMessage;

    [ObservableProperty]
    private bool hasValidationError;

    public bool IsNotBusy => !IsBusy;

    public bool CanSave => !IsBusy && SelectedCustomer != null && !HasValidationError;

    public CustomerViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
    }

    // Existing LoadCustomers command
    [RelayCommand]
    private async Task LoadCustomers()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var customerDtos = await _customerService.GetCustomersAsync();
            Customers.Clear();
            foreach (var customer in customerDtos)
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
            IsBusy = false;
        }
    }

    // Existing Refresh command
    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await LoadCustomers();
        IsRefreshing = false;
    }

    // Updated SaveCustomer command with validation
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveCustomer()
    {
        if (SelectedCustomer == null) return;

        // Validate the customer
        if (!ValidateCustomer())
        {
            return;
        }

        try
        {
            IsBusy = true;
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
                var existingCustomer = Customers.FirstOrDefault<CustomerDto>(c => c.Id == SelectedCustomer.Id);
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

            HasValidationError = false;
            ValidationMessage = string.Empty;
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
            IsBusy = false;
        }
    }

    private bool ValidateCustomer()
    {
        if (SelectedCustomer == null)
        {
            HasValidationError = true;
            ValidationMessage = "Customer information is missing";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedCustomer.FirstName) ||
            string.IsNullOrWhiteSpace(SelectedCustomer.LastName) ||
            string.IsNullOrWhiteSpace(SelectedCustomer.Email) ||
            string.IsNullOrWhiteSpace(SelectedCustomer.PhoneNumber))
        {
            HasValidationError = true;
            ValidationMessage = "Please fill in all required fields";
            return false;
        }

        HasValidationError = false;
        ValidationMessage = string.Empty;
        return true;
    }


    [RelayCommand]
    private async Task Cancel()
    {
        IsEditing = false;
        SelectedCustomer = null;
        await _navigationService.GoBackAsync();
    }

    public async Task LoadCustomer(Guid customerId)
    {
        try
        {
            IsBusy = true;
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer != null)
            {
                SelectedCustomer = customer;
                IsEditing = true;
                Title = "Edit Customer";
            }
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", "Failed to load customer");
            System.Diagnostics.Debug.WriteLine($"Load Customer Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void InitializeNewCustomer()
    {
        SelectedCustomer = new CustomerDto();
        IsEditing = false;
        Title = "New Customer";
    }

    public void OnNavigatedTo()
    {
        // Additional initialization if needed when navigating to the page
    }

    partial void OnSearchQueryChanged(string value)
    {
        FilterCustomers();
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