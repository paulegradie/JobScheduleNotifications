
namespace Mobile.Pages;

public partial class CustomersPage : ContentPage
{
    public CustomersPage(CustomersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is CustomersViewModel viewModel)
        {
            viewModel.LoadCustomersCommand.Execute(null);
        }
    }
} 