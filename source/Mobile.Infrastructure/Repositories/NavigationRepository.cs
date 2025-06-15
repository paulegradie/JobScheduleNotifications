using Mobile.UI.RepositoryAbstractions;

namespace Mobile.Infrastructure.Repositories;

/// <summary>
/// Implementation of navigation repository using MAUI Shell navigation
/// </summary>
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
}