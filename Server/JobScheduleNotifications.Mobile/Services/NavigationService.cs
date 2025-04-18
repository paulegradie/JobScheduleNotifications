namespace JobScheduleNotifications.Mobile.Services;

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters != null)
        {
            await Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current!.MainPage!.DisplayAlert(title, message, "OK");
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await Application.Current!.MainPage!.DisplayAlert(title, message, "Yes", "No");
    }
} 