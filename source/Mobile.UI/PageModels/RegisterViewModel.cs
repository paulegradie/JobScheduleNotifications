using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth.Contracts;

namespace Mobile.UI.PageModels;

public partial class RegisterViewModel : ObservableValidator
{
    private readonly IServerClient _serverClient;
    private readonly INavigationRepository _navigationUtility;

    public RegisterViewModel(IServerClient serverClient, INavigationRepository navigationUtility)
    {
        _serverClient = serverClient;
        _navigationUtility = navigationUtility;
        Title = "Register";
    }

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    private string _password = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Please confirm your password")]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Business name is required")]
    [MinLength(2, ErrorMessage = "Business name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Business name cannot exceed 100 characters")]
    private string _businessName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "First name is required")]
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters long")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Last name is required")]
    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Business address is required")]
    [MinLength(5, ErrorMessage = "Business address must be at least 5 characters long")]
    [MaxLength(200, ErrorMessage = "Business address cannot exceed 200 characters")]
    private string _businessAddress = string.Empty;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "Business description cannot exceed 500 characters")]
    private string _businessDescription = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isPasswordVisible;

    [ObservableProperty]
    private bool _isConfirmPasswordVisible;

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible;

    [RelayCommand]
    private Task Login() => NavigateToLoginAsync();

    [RelayCommand]
    private async Task Register()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || 
                string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(BusinessName) ||
                string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(BusinessAddress))
            {
                ErrorMessage = "Please fill in all required fields";
                return;
            }

            // Validate password match
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            // Validate password strength
            if (Password.Length < 8)
            {
                ErrorMessage = "Password must be at least 8 characters long";
                return;
            }

            var registration = new RegisterNewAdminRequest
            {
                Email = Email,
                Password = Password,
                BusinessName = BusinessName,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                BusinessDescription = BusinessDescription
            };

            var success = await _serverClient.Auth.RegisterAsync(registration, CancellationToken.None);
            if (success.IsSuccess)
            {
                var suc = await _serverClient.Auth.LoginAsync(new SignInRequest(Email, Password), CancellationToken.None);
                if (true)
                {
                    await _navigationUtility.NavigateToAsync(nameof(HomePage));
                }
                else
                {
                    ErrorMessage = "Registration successful but login failed. Please try logging in manually.";
                    Login();
                }
            }
            else
            {
                ErrorMessage = "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred during registration. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NavigateToLoginAsync()
    {
        await _navigationUtility.GoBackAsync();
    }
}