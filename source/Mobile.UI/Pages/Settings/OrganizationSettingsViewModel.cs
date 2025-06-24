using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.OrganizationSettings.Contracts;

namespace Mobile.UI.Pages.Settings;

public partial class OrganizationSettingsViewModel : BaseViewModel
{
    private readonly IOrganizationSettingsRepository _settingsRepository;

    [ObservableProperty] private string _businessName = string.Empty;
    [ObservableProperty] private string _businessDescription = string.Empty;
    [ObservableProperty] private string _businessIdNumber = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _streetAddress = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _state = string.Empty;
    [ObservableProperty] private string _postalCode = string.Empty;
    [ObservableProperty] private string _country = string.Empty;
    [ObservableProperty] private string _bankName = string.Empty;
    [ObservableProperty] private string _bankBsb = string.Empty;
    [ObservableProperty] private string _bankAccountNumber = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;

    [ObservableProperty] private bool _hasUnsavedChanges;

    public OrganizationSettingsViewModel(IOrganizationSettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        
        // Watch for changes to mark as dirty
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(HasUnsavedChanges) && 
                e.PropertyName != nameof(IsBusy) && 
                e.PropertyName != nameof(ErrorMessage))
            {
                HasUnsavedChanges = true;
            }
        };
    }

    public async Task InitializeAsync()
    {
        await RunWithSpinner(async () =>
        {
            try
            {
                var result = await _settingsRepository.GetOrganizationSettingsAsync();
                if (result.IsSuccess)
                {
                    var settings = result.Value;
                    BusinessName = settings.BusinessName;
                    BusinessDescription = settings.BusinessDescription;
                    BusinessIdNumber = settings.BusinessIdNumber;
                    Email = settings.Email;
                    PhoneNumber = settings.PhoneNumber;
                    StreetAddress = settings.StreetAddress;
                    City = settings.City;
                    State = settings.State;
                    PostalCode = settings.PostalCode;
                    Country = settings.Country;
                    BankName = settings.BankName;
                    BankBsb = settings.BankBsb;
                    BankAccountNumber = settings.BankAccountNumber;
                    BankAccountName = settings.BankAccountName;

                    HasUnsavedChanges = false;
                }
                else
                {
                    // Handle authorization or server errors gracefully
                    var errorMessage = result.StatusCode == System.Net.HttpStatusCode.Forbidden
                        ? "You don't have permission to access organization settings. Please contact your organization owner."
                        : result.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        ? "Server error occurred. Please try again later or contact support."
                        : $"Failed to load settings: {result.ErrorMessage}";

                    // Log the error but don't show a dialog that might crash
                    System.Diagnostics.Debug.WriteLine($"Organization settings error: {errorMessage}");

                    // Set default values so the page doesn't crash
                    Country = "Australia";
                    HasUnsavedChanges = false;
                }
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors to prevent crashes
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading organization settings: {ex.Message}");

                // Set default values
                Country = "Australia";
                HasUnsavedChanges = false;
            }
        });
    }

    public bool CanSave => !IsBusy && HasUnsavedChanges && IsValid;

    private bool IsValid
    {
        get
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(this);
            return Validator.TryValidateObject(this, context, validationResults, true);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await RunWithSpinner(async () =>
        {
            var request = new UpdateOrganizationSettingsRequest(
                businessName: BusinessName,
                businessDescription: BusinessDescription,
                businessIdNumber: BusinessIdNumber,
                email: Email,
                phoneNumber: PhoneNumber,
                streetAddress: StreetAddress,
                city: City,
                state: State,
                postalCode: PostalCode,
                country: Country,
                bankName: BankName,
                bankBsb: BankBsb,
                bankAccountNumber: BankAccountNumber,
                bankAccountName: BankAccountName);

            var result = await _settingsRepository.UpdateOrganizationSettingsAsync(request);
            if (result.IsSuccess)
            {
                HasUnsavedChanges = false;
                await ShowSuccessAsync("Settings saved successfully!");
            }
            else
            {
                // Handle authorization or server errors gracefully
                var errorMessage = result.StatusCode == System.Net.HttpStatusCode.Forbidden
                    ? "You don't have permission to update organization settings. Please contact your organization owner."
                    : result.StatusCode == System.Net.HttpStatusCode.InternalServerError
                    ? "Server error occurred while saving. Please try again later or contact support."
                    : $"Failed to save settings: {result.ErrorMessage ?? "Unknown error occurred"}";

                // Log the error but try to show a safe error message
                System.Diagnostics.Debug.WriteLine($"Save organization settings error: {errorMessage}");

                try
                {
                    await ShowErrorAsync("Failed to save settings", errorMessage);
                }
                catch (Exception ex)
                {
                    // If showing the error dialog fails, just log it
                    System.Diagnostics.Debug.WriteLine($"Failed to show error dialog: {ex.Message}");
                }
            }
        });
    }

    partial void OnBusinessNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBusinessDescriptionChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBusinessIdNumberChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnEmailChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnPhoneNumberChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnStreetAddressChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnCityChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnStateChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnPostalCodeChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnCountryChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBankNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBankBsbChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBankAccountNumberChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnBankAccountNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    partial void OnHasUnsavedChangesChanged(bool value) => SaveCommand.NotifyCanExecuteChanged();
}
