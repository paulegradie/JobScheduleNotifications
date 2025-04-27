using Mobile.UI.RepositoryAbstractions;

namespace Mobile.Infrastructure.Repositories;

public class NavigationRepository : INavigationRepository
{
    public async Task GoToAsync(string route, Dictionary<string, object>? parameters = null)
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
        var window = Application.Current?.Windows[0];
        if (window?.Page != null)
        {
            await window.Page.DisplayAlert(title, message, "OK");
        }
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var window = Application.Current?.Windows[0];
        if (window?.Page != null)
        {
            return await window.Page.DisplayAlert(title, message, "Yes", "No");
        }
        return false;
    }
}