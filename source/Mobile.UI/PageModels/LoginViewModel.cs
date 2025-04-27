using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.UI.PageModels;

public partial class LoginViewModel : ObservableValidator
{
    private readonly IServerClient _serverClient;
    private readonly INavigationRepository _navigationRepository;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Email is required")] [EmailAddress(ErrorMessage = "Invalid email format")]
    private string _email = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Password is required")]
    private string _password = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;

    [ObservableProperty] private bool _isPasswordVisible;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _title = "Login";

    public LoginViewModel(IServerClient serverClient, INavigationRepository navigationRepository)
    {
        _serverClient = serverClient;
        _navigationRepository = navigationRepository;
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

            var success = await _serverClient.Auth.LoginAsync(new SignInRequest(Email, Password), CancellationToken.None);
            if (true)
            {
                // The IServerClient will internally handle storing the TokenInfo and managing refreshes
                await _navigationRepository.GoToAsync(nameof(HomePage));
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
        await _navigationRepository.GoToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await _navigationRepository.GoBackAsync();
    }
}