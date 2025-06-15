namespace Mobile.UI.RepositoryAbstractions;

/// <summary>
/// Repository abstraction for navigation functionality
/// </summary>
public interface INavigationRepository
{
    /// <summary>
    /// Navigate to a specific route with optional parameters
    /// </summary>
    /// <param name="route">The route to navigate to</param>
    /// <param name="parameters">Optional parameters to pass to the destination page</param>
    Task GoToAsync(string route, Dictionary<string, object>? parameters = null);

    /// <summary>
    /// Navigate back to the previous page
    /// </summary>
    Task GoBackAsync();
}