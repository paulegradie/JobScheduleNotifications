using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mobile.PageModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _count;

        [ObservableProperty]
        private string _counterText = "Click me";

        [RelayCommand]
        private void IncrementCounter()
        {
            Count++;
            CounterText = Count == 1 ? $"Clicked {Count} time" : $"Clicked {Count} times";
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync(nameof(Pages.LoginPage));
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(Pages.RegisterPage));
        }
    }
} 