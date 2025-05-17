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
            ErrorMessage = errorMessage ?? ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> RunWithSpinner<T>(Func<Task<T>> operation, string? errorMessage = null)
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
            ErrorMessage = errorMessage ?? ex.Message;
        }
        finally
        {
            IsBusy = false;
        }

        return default;
    }
}