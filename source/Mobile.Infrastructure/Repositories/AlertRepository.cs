using Mobile.UI.RepositoryAbstractions;

namespace Mobile.Infrastructure.Repositories;

/// <summary>
/// Implementation of alert repository that displays alerts and confirmations using MAUI's DisplayAlert
/// </summary>
public class AlertRepository : IAlertRepository
{
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
