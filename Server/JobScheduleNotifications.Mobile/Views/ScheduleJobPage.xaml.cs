using JobScheduleNotifications.Mobile.ViewModels;

namespace JobScheduleNotifications.Mobile.Views;

public partial class ScheduleJobPage : ContentPage
{
    public ScheduleJobPage(ScheduleJobViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ScheduleJobViewModel viewModel)
        {
            viewModel.LoadCustomersCommand.Execute(null);
        }
    }
} 