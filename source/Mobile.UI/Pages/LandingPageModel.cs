using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Customers;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.UI.Pages;

public partial class LandingPageModel : BaseViewModel
{
    private readonly INavigationRepository _navigation;
    private readonly IServerClient _serverClient;
    private readonly IAuthClient _authClient;
    private readonly ITokenRepository _tokenRepository;

    public LandingPageModel(
        INavigationRepository navigation,
        IServerClient serverClient,
        IAuthClient authClient,
        ITokenRepository tokenRepository)
    {
        _navigation = navigation;
        _serverClient = serverClient;
        _authClient = authClient;
        _tokenRepository = tokenRepository;
    }

    [ObservableProperty] private int _count;
    [ObservableProperty] private string _counterText = "Click me";


    [RelayCommand]
    private void IncrementCounter()
    {
        Count++;
        CounterText = Count == 1 ? $"Clicked {Count} time" : $"Clicked {Count} times";
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    [RelayCommand]
    private async Task NavigateToRegister()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private async Task NavigateToCustomers()
    {
        await _navigation.GoToAsync(nameof(CustomerListPage));
    }

    [RelayCommand]
    public async Task AutoLoginForDev()
    {
#if DEBUG
        // — your existing registration + login —
        var newTestRego = new RegisterNewAdminRequest
        {
            Email = $"{Guid.NewGuid().ToString()}@gmail.com",
            Password = "TestPassword123!",
            BusinessName = "Paul",
            FirstName = "Paul",
            LastName = "Gradie",
            PhoneNumber = "8607530466",
            BusinessDescription = "Demo Business"
        };
        var registered = await _authClient.Auth.RegisterAsync(newTestRego, CancellationToken.None);
        if (!registered.IsSuccess)
            throw new InvalidOperationException($"Dev signup failed: {registered.ErrorMessage}");

        var token = await _authClient.Auth.LoginAsync(
            new SignInRequest(newTestRego.Email, newTestRego.Password),
            CancellationToken.None);
        if (!token.IsSuccess)
            throw new Exception($"Dev login failed: {token.ErrorMessage}");

        // persist & wire up the bearer header
        await _tokenRepository.StoreTokenMeta(token.Value);
        _serverClient.Http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Value.AccessToken);

        // — now navigate to Dashboard —
        await _navigation.GoToAsync(nameof(CustomerListPage));
#endif
    }
}