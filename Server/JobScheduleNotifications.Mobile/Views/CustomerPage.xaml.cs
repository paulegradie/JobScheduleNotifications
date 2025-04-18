using JobScheduleNotifications.Mobile.ViewModels;

namespace JobScheduleNotifications.Mobile.Views;

[QueryProperty(nameof(CustomerId), "CustomerId")]
public partial class CustomerPage : ContentPage
{
    private Guid? _customerId;
    public Guid? CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            if (BindingContext is CustomerEditViewModel viewModel)
            {
                if (value.HasValue)
                {
                    viewModel.Initialize(value.Value);
                }
                else
                {
                    viewModel.Initialize();
                }
            }
        }
    }

    public CustomerPage(CustomerEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 