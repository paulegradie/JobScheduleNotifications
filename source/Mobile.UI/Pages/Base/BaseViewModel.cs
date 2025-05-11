// BaseViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;

namespace Mobile.UI.Pages.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    // common “busy”/“error” lifecycle state
    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _errorMessage = string.Empty;

    // optional: a base “Refresh” command pattern
    protected async Task RunWithSpinner(Func<Task> operation, string? errorMessage = null)
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
            if (errorMessage != null)
            {
                ErrorMessage = errorMessage;
            }
            else
            {
                ErrorMessage = ex.Message;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}