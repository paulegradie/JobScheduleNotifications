using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Navigation;

namespace Mobile.UI.Pages.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Sets the service provider for all BaseViewModel instances.
    /// This should be called once during application startup.
    /// </summary>
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the AlertRepository instance from the DI container
    /// </summary>
    protected IAlertRepository AlertRepository =>
        _serviceProvider?.GetRequiredService<IAlertRepository>()
        ?? throw new InvalidOperationException("Service provider not set. Call BaseViewModel.SetServiceProvider() during app startup.");

    /// <summary>
    /// Gets the TypeSafeNavigationRepository instance from the DI container for type-safe navigation
    /// </summary>
    protected ITypeSafeNavigationRepository Navigation =>
        _serviceProvider?.GetRequiredService<ITypeSafeNavigationRepository>()
        ?? throw new InvalidOperationException("Service provider not set. Call BaseViewModel.SetServiceProvider() during app startup.");

    /// <summary>
    /// Shows an alert dialog with the specified title and message
    /// </summary>
    protected async Task ShowAlertAsync(string title, string message)
    {
        await AlertRepository.ShowAlertAsync(title, message);
    }

    /// <summary>
    /// Shows a confirmation dialog and returns the user's choice
    /// </summary>
    protected async Task<bool> ShowConfirmationAsync(string title, string message = "")
    {
        return await AlertRepository.ShowConfirmationAsync(title, message);
    }

    /// <summary>
    /// Shows an error alert with a standard "Error" title
    /// </summary>
    protected async Task ShowErrorAsync(string message)
    {
        await ShowErrorAsync("Error", message);
    }

    protected async Task ShowErrorAsync(string title, string message)
    {
        await AlertRepository.ShowAlertAsync(title, message);
    }

    /// <summary>
    /// Shows a success alert with a standard "Success" title
    /// </summary>
    protected async Task ShowSuccessAsync(string message)
    {
        await ShowSuccessAsync("Success", message);
    }

    protected async Task ShowSuccessAsync(string title, string message)
    {
        await AlertRepository.ShowAlertAsync("Success", message);
    }

    /// <summary>
    /// Gets the NavigationRepository instance from the DI container for basic navigation
    /// </summary>
    private INavigationRepository BasicNavigation =>
        _serviceProvider?.GetRequiredService<INavigationRepository>()
        ?? throw new InvalidOperationException("Service provider not set. Call BaseViewModel.SetServiceProvider() during app startup.");

    /// <summary>
    /// Navigate to a specific route with optional parameters
    /// </summary>
    protected async Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
    {
        await BasicNavigation.GoToAsync(route, parameters);
    }

    // common “busy”/“error” lifecycle state
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    protected async Task RunWithSpinner(Func<Task> operation, string? errorMessage = null, Action<Exception>? errorCallback = null, bool showErrorAlert = false)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            var displayMessage = errorMessage ?? ex.Message;
            ErrorMessage = displayMessage;

            if (showErrorAlert)
            {
                await ShowErrorAsync(displayMessage);
            }

            errorCallback?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> RunWithSpinner<T>(Func<Task<T>> operation, string? errorMessage = null, Action<Exception>? errorCallback = null, bool showErrorAlert = false)
    {
        if (IsBusy) return default;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            var displayMessage = errorMessage ?? ex.Message;
            ErrorMessage = displayMessage;

            if (showErrorAlert)
            {
                await ShowErrorAsync(displayMessage);
            }

            errorCallback?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
        }

        return default;
    }

    [RelayCommand]
    protected async Task GoBackAsync()
    {
        await Navigation.GoBackAsync();
    }
}