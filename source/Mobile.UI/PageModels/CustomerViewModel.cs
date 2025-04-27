using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.PageModels;

public partial class CustomerViewModel : ObservableValidator
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigationUtility;

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

    public CustomerViewModel(ICustomerRepository customerRepository, INavigationRepository navigationUtility)
    {
        _customerRepository = customerRepository;
        _navigationUtility = navigationUtility;
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
                var update = UpdateCustomerRequest.CreateBuilder(CustomerId)
                    .WithFirstName(SelectedCustomer.FirstName)
                    .WithLastName(SelectedCustomer.LastName)
                    .WithPhoneNumber(SelectedCustomer.PhoneNumber)
                    .WithNotes(SelectedCustomer.Notes)
                    .Build();

                await _customerRepository.UpdateCustomerAsync(update, CancellationToken.None);
                var existingCustomer = Customers.FirstOrDefault<CustomerDto>(c => c.Id == SelectedCustomer.Id);
                if (existingCustomer != null)
                {
                    var index = Customers.IndexOf(existingCustomer);
                    Customers[index] = SelectedCustomer;
                }
            }
            else
            {
                var createCustomer = new CreateCustomerRequest(
                    firstName: SelectedCustomer.FirstName,
                    lastName: SelectedCustomer.LastName,
                    email: SelectedCustomer.Email,
                    phoneNumber: SelectedCustomer.PhoneNumber,
                    notes: SelectedCustomer.Notes);

                var newCustomer = await _customerRepository.CreateCustomerAsync(createCustomer);

                if (!newCustomer.IsSuccess) throw new Exception("Couldn't create customer");
                Customers.Add(newCustomer.Value);
            }

            HasValidationError = false;
            ValidationMessage = string.Empty;
            IsEditing = false;
            SelectedCustomer = null;
            await _navigationUtility.GoBackAsync();
        }
        catch (Exception ex)
        {
            await _navigationUtility.ShowAlertAsync("Error", "Failed to save customer");
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
        await _navigationUtility.GoBackAsync();
    }

    [RelayCommand]
    public async Task LoadCustomer(Guid customerId)
    {
        try
        {
            IsBusy = true;
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer is { IsSuccess: true, Value: not null })
            {
                SelectedCustomer = customer.Value;
                IsEditing = true;
                Title = "Edit Customer";
            }
        }
        catch (Exception ex)
        {
            await _navigationUtility.ShowAlertAsync("Error", "Failed to load customer");
            System.Diagnostics.Debug.WriteLine($"Load Customer Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public string Name { get; set; } = "Not Set Yet";
    public CustomerId CustomerId { get; set; }
}