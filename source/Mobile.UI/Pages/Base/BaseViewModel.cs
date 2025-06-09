using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Mobile.UI.Pages.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    // common “busy”/“error” lifecycle state
    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _errorMessage = string.Empty;

    protected async Task RunWithSpinner(Func<Task> operation, string? errorMessage = null, Action<Exception>? errorCallback = null)
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
            errorCallback?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> RunWithSpinner<T>(Func<Task<T>> operation, string? errorMessage = null, Action<Exception>? errorCallback = null)
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
            errorCallback?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
        }

        return default;
    }
}