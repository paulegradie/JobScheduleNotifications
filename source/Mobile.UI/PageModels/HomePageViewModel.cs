using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Services;
using Mobile.UI.Pages;

namespace Mobile.UI.PageModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly INavigationUtility _navigation;

        public HomePageViewModel(INavigationUtility navigation)
        {
            _navigation = navigation;
        }

        [ObservableProperty] private int _count;
        [ObservableProperty] private string _counterText = "Click me";


        [RelayCommand]
        private void IncrementCounter()
        {
            Count++;
            CounterText = Count == 1 ? $"Clicked {Count} time" : $"Clicked {Count} times";
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        private async Task NavigateToCustomers()
        {
            await _navigation.NavigateToAsync(nameof(CustomersPage));
        }


    }
}