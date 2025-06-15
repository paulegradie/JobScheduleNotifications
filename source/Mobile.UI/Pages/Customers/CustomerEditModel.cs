using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerEditModel : BaseViewModel
{
    private readonly ICustomerRepository _repository;
    public CustomerId CustomerId { get; set; }

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private string _title = "Add Customer";

    public CustomerEditModel(ICustomerRepository repository)
    {
        _repository = repository;
        // Subscribe to IsBusy changes to update CanSave
        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
            {
                SaveCustomerCommand.NotifyCanExecuteChanged();
            }
        };
    }


    [RelayCommand]
    private async Task LoadCustomerAsync(string customerId)
    {
        CustomerId = CustomerId.Parse(customerId);
        await RunWithSpinner(async () =>
        {
            var result = await _repository.GetCustomerByIdAsync(CustomerId);
            if (result is { IsSuccess: true, Value: not null })
            {
                FirstName = result.Value.FirstName;
                LastName = result.Value.LastName;
                Email = result.Value.Email;
                PhoneNumber = result.Value.PhoneNumber;
                Notes = result.Value.Notes;

                // Notify that CanSave should be re-evaluated after loading data
                SaveCustomerCommand.NotifyCanExecuteChanged();
            }
            else
            {
                await ShowErrorAsync($"Failed to load customer: {result.ErrorMessage ?? "Customer not found"}");
            }
        }, "Failed to load customer");
    }

    // Validation properties for individual fields
    public bool IsFirstNameValid => !string.IsNullOrWhiteSpace(FirstName);
    public bool IsLastNameValid => !string.IsNullOrWhiteSpace(LastName);
    public bool IsEmailValid => !string.IsNullOrWhiteSpace(Email);
    public bool IsPhoneNumberValid => !string.IsNullOrWhiteSpace(PhoneNumber);

    // Validation error messages
    public string FirstNameError => IsFirstNameValid ? string.Empty : "First name is required";
    public string LastNameError => IsLastNameValid ? string.Empty : "Last name is required";
    public string EmailError => IsEmailValid ? string.Empty : "Email address is required";
    public string PhoneNumberError => IsPhoneNumberValid ? string.Empty : "Phone number is required";

    // Overall validation message
    public string ValidationMessage
    {
        get
        {
            if (CanSave) return string.Empty;
            if (IsBusy) return "Please wait...";

            var missingFields = new List<string>();
            if (!IsFirstNameValid) missingFields.Add("First Name");
            if (!IsLastNameValid) missingFields.Add("Last Name");
            if (!IsEmailValid) missingFields.Add("Email");
            if (!IsPhoneNumberValid) missingFields.Add("Phone Number");

            return missingFields.Count > 0
                ? $"Please fill in: {string.Join(", ", missingFields)}"
                : string.Empty;
        }
    }

    public bool CanSave
        => !IsBusy
           && IsFirstNameValid
           && IsLastNameValid
           && IsEmailValid
           && IsPhoneNumberValid;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveCustomerAsync()
    {
        await RunWithSpinner(async () =>
        {
            var update = UpdateCustomerRequest.CreateBuilder(CustomerId)
                .WithFirstName(FirstName)
                .WithLastName(LastName)
                .WithEmail(Email)
                .WithPhoneNumber(PhoneNumber)
                .WithNotes(Notes)
                .Build();

            var result = await _repository.UpdateCustomerAsync(update, CancellationToken.None);

            if (result.IsSuccess)
            {
                await ShowSuccessAsync("Customer Updated", "Customer information has been updated successfully!");
                await Navigation.NavigateToCustomerListAsync();
            }
            else
            {
                await ShowErrorAsync($"Failed to update customer: {result.ErrorMessage ?? "Unknown error"}");
            }
        }, "Unable to update customer");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        try
        {
            await Navigation.NavigateToCustomerListAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Navigation error: {ex.Message}");
        }
    }

    // Property change notifications to update CanSave and validation when form fields change
    partial void OnFirstNameChanged(string oldValue, string newValue)
    {
        SaveCustomerCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsFirstNameValid));
        OnPropertyChanged(nameof(FirstNameError));
        OnPropertyChanged(nameof(ValidationMessage));
    }

    partial void OnLastNameChanged(string oldValue, string newValue)
    {
        SaveCustomerCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsLastNameValid));
        OnPropertyChanged(nameof(LastNameError));
        OnPropertyChanged(nameof(ValidationMessage));
    }

    partial void OnEmailChanged(string oldValue, string newValue)
    {
        SaveCustomerCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsEmailValid));
        OnPropertyChanged(nameof(EmailError));
        OnPropertyChanged(nameof(ValidationMessage));
    }

    partial void OnPhoneNumberChanged(string oldValue, string newValue)
    {
        SaveCustomerCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsPhoneNumberValid));
        OnPropertyChanged(nameof(PhoneNumberError));
        OnPropertyChanged(nameof(ValidationMessage));
    }


}