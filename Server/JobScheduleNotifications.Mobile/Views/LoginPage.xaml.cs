using JobScheduleNotifications.Mobile.ViewModels;

namespace JobScheduleNotifications.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 