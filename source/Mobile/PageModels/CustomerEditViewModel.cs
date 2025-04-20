using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JobScheduleNotifications.Contracts.Customers;

namespace Mobile.PageModels;

public partial class CustomerEditViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;
    private Guid? _customerId;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = "Add Customer";

    public CustomerEditViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
    }

    public void Initialize(Guid? customerId = null)
    {
        _customerId = customerId;
        Title = customerId.HasValue ? "Edit Customer" : "Add Customer";
        
        if (customerId.HasValue)
        {
            MainThread.BeginInvokeOnMainThread(async () => await LoadCustomerAsync(customerId.Value));
        }
    }

    private async Task LoadCustomerAsync(Guid id)
    {
        try
        {
            IsBusy = true;
            var customer = await _customerService.GetCustomerByIdAsync(id);
            
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
            PhoneNumber = customer.PhoneNumber;
            Notes = customer.Notes;
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", $"Failed to load customer: {ex.Message}");
            await _navigationService.GoBackAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveCustomer()
    {
        try
        {
            IsBusy = true;

            if (_customerId.HasValue)
            {
                var updateDto = new UpdateCustomerDto
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Notes = Notes
                };

                await _customerService.UpdateCustomerAsync(_customerId.Value, updateDto);
                await _navigationService.ShowAlertAsync("Success", "Customer updated successfully");
            }
            else
            {
                var createDto = new CreateCustomerDto
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Notes = Notes
                };

                await _customerService.CreateCustomerAsync(createDto);
                await _navigationService.ShowAlertAsync("Success", "Customer created successfully");
            }

            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", $"Failed to save customer: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigationService.GoBackAsync();
    }
} 