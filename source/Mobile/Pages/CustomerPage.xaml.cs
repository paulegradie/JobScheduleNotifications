using System.Diagnostics;

namespace Mobile.Pages;

public abstract partial class BasePage<TViewModel>(TViewModel viewModel) : BasePage(viewModel)
{
    protected new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

public abstract class BasePage : ContentPage
{
    protected BasePage(object? viewModel = null)
    {
        BindingContext = viewModel;
        Padding = 12;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Trace.WriteLine($"OnAppearing: {Title}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Trace.WriteLine($"OnDisappearing: {Title}");
    }
}

public partial class CustomerPage(CustomerViewModel viewModel) : BasePage<CustomerViewModel>(viewModel)
{
    private Guid? _customerId;

    public Guid? CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            if (BindingContext is CustomerViewModel viewModel)
            {
                if (value.HasValue)
                {
                    MainThread.BeginInvokeOnMainThread(() => viewModel.LoadCustomer(value.Value).Wait());
                }
                else
                {
                    viewModel.InitializeNewCustomer();
                }
            }
        }
    }
    

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is CustomerViewModel viewModel)
        {
            viewModel.OnNavigatedTo();
        }
    }
}