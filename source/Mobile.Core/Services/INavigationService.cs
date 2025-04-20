namespace Mobile.Core.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task GoBackAsync();
    Task ShowAlertAsync(string title, string message);
    Task<bool> ShowConfirmationAsync(string title, string message);
} 