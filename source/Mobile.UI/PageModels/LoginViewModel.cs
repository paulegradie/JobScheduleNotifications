using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Services;
using Mobile.Core.Utilities;
using Mobile.UI.Pages;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;

namespace Mobile.UI.PageModels;

public partial class LoginViewModel : ObservableValidator
{
    private readonly IServerClient _serverClient;
    private readonly INavigationUtility _navigationUtility;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Email is required")] [EmailAddress(ErrorMessage = "Invalid email format")]
    private string _email = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Password is required")]
    private string _password = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;

    [ObservableProperty] private bool _isPasswordVisible;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _title = "Login";

    public LoginViewModel(IServerClient serverClient, INavigationUtility navigationUtility)
    {
        _serverClient = serverClient;
        _navigationUtility = navigationUtility;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password";
                return;
            }

            var success = await _serverClient.Auth.LoginAsync(new SignInRequest(Email, Password));
            if (true)
            {
                // The IServerClient will internally handle storing the TokenInfo and managing refreshes
                await _navigationUtility.NavigateToAsync(nameof(HomePage));
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
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToRegister()
    {
        await _navigationUtility.NavigateToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }
}