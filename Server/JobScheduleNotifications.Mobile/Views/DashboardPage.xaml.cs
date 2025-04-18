using JobScheduleNotifications.Mobile.ViewModels;

namespace JobScheduleNotifications.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is DashboardViewModel viewModel)
        {
            viewModel.LoadDashboardDataCommand.Execute(null);
        }
    }
}