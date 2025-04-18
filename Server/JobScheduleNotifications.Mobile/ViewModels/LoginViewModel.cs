using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using JobScheduleNotifications.Mobile.Services;
using JobScheduleNotifications.Mobile.Views;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isPasswordVisible;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Login";
        LoginCommand = new Command(async () => await LoginAsync());
        RegisterCommand = new Command(async () => await NavigateToRegisterAsync());
        TogglePasswordVisibilityCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
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
    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
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

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand TogglePasswordVisibilityCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password";
                return;
            }

            var success = await _authService.LoginAsync(Email, Password);
            if (success)
            {
                await _navigationService.NavigateToAsync(nameof(DashboardPage));
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred during login. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task NavigateToRegisterAsync()
    {
        await _navigationService.NavigateToAsync(nameof(RegisterPage));
    }
} 