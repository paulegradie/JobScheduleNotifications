using JobScheduleNotifications.Mobile.ViewModels;

namespace JobScheduleNotifications.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 