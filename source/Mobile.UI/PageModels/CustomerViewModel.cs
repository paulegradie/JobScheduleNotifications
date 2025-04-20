using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Services;
using Server.Contracts.Customers;
using ICustomerService = Mobile.Core.Interfaces.ICustomerService;

namespace Mobile.UI.PageModels;

public partial class CustomerViewModel : ObservableValidator
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

    [ObservableProperty] private string _searchQuery = string.Empty;

    [ObservableProperty] private bool _isRefreshing;

    [ObservableProperty] private bool _isEditing;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))] [NotifyPropertyChangedFor(nameof(CanSave))]
    private bool _isBusy;

    [ObservableProperty] private string _title = "Customers";

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCustomerCommand))]
    private CustomerDto? _selectedCustomer;

    [ObservableProperty] private string? _validationMessage;

    [ObservableProperty] private bool _hasValidationError;

    public bool IsNotBusy => !IsBusy;

    public bool CanSave => !IsBusy && SelectedCustomer != null && !HasValidationError;

    public CustomerViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
    }


    // Updated SaveCustomer command with validation
    [RelayCommand(CanExecute = nameof(CanSave))]
    public async Task SaveCustomer()
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
                var existingCustomer = Enumerable.FirstOrDefault<CustomerDto>(Customers, c => c.Id == SelectedCustomer.Id);
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
    public async Task Cancel()
    {
        IsEditing = false;
        SelectedCustomer = null;
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
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

    public string Name { get; set; } = "Not Set Yet";
    public Guid CustomerId { get; set; }
}