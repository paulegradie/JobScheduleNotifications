using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.PageModels;

public partial class CustomerEditViewModel : ObservableObject
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigationRepository;
    private CustomerId? _customerId;

    [ObservableProperty] private string _firstName = string.Empty;

    [ObservableProperty] private string _lastName;

    [ObservableProperty] private string _email = string.Empty;

    [ObservableProperty] private string _phoneNumber = string.Empty;

    [ObservableProperty] private string _notes = string.Empty;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _title = "Add Customer";

    public CustomerEditViewModel(ICustomerRepository customerRepository, INavigationRepository navigationRepository)
    {
        _customerRepository = customerRepository;
        _navigationRepository = navigationRepository;
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
            var result = await _customerRepository.GetCustomerByIdAsync(id);
            var customer = result.Value;

            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
            PhoneNumber = customer.PhoneNumber;
            Notes = customer.Notes;
        }
        catch (Exception ex)
        {
            await _navigationRepository.ShowAlertAsync("Error", $"Failed to load customer: {ex.Message}");
            await _navigationRepository.GoBackAsync();
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
                var update = UpdateCustomerRequest.CreateBuilder(_customerId.Value)
                    .WithFirstName(FirstName)
                    .WithLastName(LastName)
                    .WithEmail(Email)
                    .WithPhoneNumber(PhoneNumber)
                    .WithNotes(Notes)
                    .Build();

                await _customerRepository.UpdateCustomerAsync(update, CancellationToken.None);
                await _navigationRepository.ShowAlertAsync("Success", "Customer updated successfully");
            }
            else
            {
                var update = new CreateCustomerRequest(
                    firstName: FirstName,
                    lastName: LastName,
                    email: Email,
                    phoneNumber: PhoneNumber,
                    notes: Notes);

                await _customerRepository.CreateCustomerAsync(update);
                await _navigationRepository.ShowAlertAsync("Success", "Customer created successfully");
            }

            await _navigationRepository.GoBackAsync();
        }
        catch (Exception ex)
        {
            await _navigationRepository.ShowAlertAsync("Error", $"Failed to save customer: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigationRepository.GoBackAsync();
    }
}