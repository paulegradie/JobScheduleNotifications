using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers;

[CustomerIdQueryParam]
public sealed class CustomerViewPage : BasePage<CustomerViewModel>
{
    public string CustomerId { get; set; }

    public CustomerViewPage(CustomerViewModel vm) : base(vm)
    {
        Title = ViewModel.Title;
        Content = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Details, Star),
                (Row.Buttons, Auto)
            ),
            Children =
            {
                // Details section
                new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 10,
                    Children =
                    {
                        new Label().Text("Name").Font(size: 14, bold: true),
                        new Label().Bind(Label.TextProperty, nameof(vm.FirstName)),

                        new Label().Text("Email").Font(size: 14, bold: true),
                        new Label().Bind(Label.TextProperty, nameof(vm.Email)),

                        new Label().Text("Phone").Font(size: 14, bold: true),
                        new Label().Bind(Label.TextProperty, nameof(vm.PhoneNumber)),

                        new Label().Text("Notes").Font(size: 14, bold: true),
                        new Label().Bind(Label.TextProperty, nameof(vm.Notes)),

                        new Label()
                            .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                            .TextColor(Colors.Red)
                            .Bind(IsVisibleProperty, nameof(vm.ErrorMessage))
                    }
                }.Row(Row.Details),

                new Button { Text = "Edit" }
                    .BindCommand(nameof(vm.EditCustomerCommand))
                    .Row(Row.Buttons),
                CardStyles.CreateSecondaryButton("New Job", CardStyles.Colors.Success)
                    .Bind(Button.CommandProperty, nameof(ViewModel.CreateJobCommand), source: ViewModel)
                    .Bind(Button.CommandParameterProperty, "."),
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Guid.TryParse(CustomerId, out var id))
        {
            ViewModel.LoadCustomerCommand.Execute(new CustomerId(id));
        }
    }


    private enum Row
    {
        Details,
        Buttons
    }
}