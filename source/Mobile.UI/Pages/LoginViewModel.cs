using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.UI.Pages;

public partial class LoginViewModel : BaseValidatorModel
{
    private readonly IServerClient _serverClient;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Email is required")] [EmailAddress(ErrorMessage = "Invalid email format")]
    private string _email = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ErrorMessage))] [Required(ErrorMessage = "Password is required")]
    private string _password = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;

    [ObservableProperty] private bool _isPasswordVisible;


    [ObservableProperty] private string _title = "Login";

    public LoginViewModel(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both email and password";
            return;
        }

        await RunWithSpinner(async () =>
        {
            var success = await _serverClient.Auth.LoginAsync(new SignInRequest(Email, Password), CancellationToken.None);
            if (true)
            {
                await Navigation.NavigateToLandingPageAsync();
            }
        }, "An error occurred during login. Please try again.");
    }

    [RelayCommand]
    private async Task NavigateToRegister()
    {
        await Navigation.NavigateToRegisterAsync();
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Navigation.NavigateToLandingPageAsync();
    }
}