using System.Windows.Input;
using JobScheduleNotifications.Contracts.Customers;
using JobScheduleNotifications.Mobile.Services;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class CustomerEditViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;
    private Guid? _customerId;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _notes = string.Empty;

    public CustomerEditViewModel(ICustomerService customerService, INavigationService navigationService)
    {
        _customerService = customerService;
        _navigationService = navigationService;
        
        SaveCommand = new Command(async () => await SaveCustomerAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public void Initialize(Guid? customerId = null)
    {
        _customerId = customerId;
        Title = customerId.HasValue ? "Edit Customer" : "Add Customer";
        
        if (customerId.HasValue)
        {
            LoadCustomerAsync(customerId.Value).ConfigureAwait(false);
        }
    }

    private async Task LoadCustomerAsync(Guid id)
    {
        try
        {
            SetBusy(true);
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
            SetBusy(false);
        }
    }

    private async Task SaveCustomerAsync()
    {
        try
        {
            SetBusy(true);

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
            SetBusy(false);
        }
    }

    private async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }
} 