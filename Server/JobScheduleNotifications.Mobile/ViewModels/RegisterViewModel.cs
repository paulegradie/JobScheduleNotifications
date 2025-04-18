using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using JobScheduleNotifications.Contracts.Authentication;
using JobScheduleNotifications.Mobile.Services;
using JobScheduleNotifications.Mobile.Views;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _businessName = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _businessAddress = string.Empty;
    private string _businessDescription = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isPasswordVisible;
    private bool _isConfirmPasswordVisible;

    public RegisterViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Register";
        RegisterCommand = new Command(ExecuteRegisterCommand);
        LoginCommand = new Command(ExecuteLoginCommand);
        TogglePasswordVisibilityCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
        ToggleConfirmPasswordVisibilityCommand = new Command(() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible);
    }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            SetProperty(ref _confirmPassword, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Business name is required")]
    [MinLength(2, ErrorMessage = "Business name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Business name cannot exceed 100 characters")]
    public string BusinessName
    {
        get => _businessName;
        set
        {
            SetProperty(ref _businessName, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "First name is required")]
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters long")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName
    {
        get => _firstName;
        set
        {
            SetProperty(ref _firstName, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Last name is required")]
    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long")]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName
    {
        get => _lastName;
        set
        {
            SetProperty(ref _lastName, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            SetProperty(ref _phoneNumber, value);
            ErrorMessage = string.Empty;
        }
    }

    [Required(ErrorMessage = "Business address is required")]
    [MinLength(5, ErrorMessage = "Business address must be at least 5 characters long")]
    [MaxLength(200, ErrorMessage = "Business address cannot exceed 200 characters")]
    public string BusinessAddress
    {
        get => _businessAddress;
        set
        {
            SetProperty(ref _businessAddress, value);
            ErrorMessage = string.Empty;
        }
    }

    [MaxLength(500, ErrorMessage = "Business description cannot exceed 500 characters")]
    public string BusinessDescription
    {
        get => _businessDescription;
        set
        {
            SetProperty(ref _businessDescription, value);
            ErrorMessage = string.Empty;
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsPasswordVisible
    {
        get => _isPasswordVisible;
        set => SetProperty(ref _isPasswordVisible, value);
    }

    public bool IsConfirmPasswordVisible
    {
        get => _isConfirmPasswordVisible;
        set => SetProperty(ref _isConfirmPasswordVisible, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand ToggleConfirmPasswordVisibilityCommand { get; }

    private void ExecuteRegisterCommand()
    {
        RegisterAsync().ConfigureAwait(false);
    }

    private void ExecuteLoginCommand()
    {
        NavigateToLoginAsync().ConfigureAwait(false);
    }

    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
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

            var registration = new RegisterBusinessOwnerDto
            {
                Email = Email,
                Password = Password,
                BusinessName = BusinessName,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                BusinessAddress = BusinessAddress,
                BusinessDescription = BusinessDescription
            };

            var success = await _authService.RegisterAsync(registration);
            if (success)
            {
                // After successful registration, log the user in
                success = await _authService.LoginAsync(Email, Password);
                if (success)
                {
                    await _navigationService.NavigateToAsync(nameof(DashboardPage));
                }
                else
                {
                    ErrorMessage = "Registration successful but login failed. Please try logging in manually.";
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
            SetBusy(false);
        }
    }

    private async Task NavigateToLoginAsync()
    {
        await _navigationService.GoBackAsync();
    }
} 