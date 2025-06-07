using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile.UI.RepositoryAbstractions;

public interface INavigationRepository
{
    Task GoToAsync(string route, Dictionary<string, object>? parameters = null);
    Task GoBackAsync();
    Task ShowAlertAsync(string title, string message);
    Task<bool> ShowConfirmationAsync(string title, string message);
}