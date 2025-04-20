using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class CustomerPage : BasePage<CustomerViewModel>
{
    private readonly CustomerViewModel _vm;

    public CustomerPage(CustomerViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = "Customer";

        Content = new Grid
        {
            RowDefinitions = Rows.Define((Row.Form, Star), (Row.Buttons, Auto)),
            Children =
            {
                new Entry().Placeholder("Name")
                    .Bind(Entry.TextProperty, nameof(vm.Name)),
                new Button { Text = "Save" }
                    .BindCommand(nameof(vm.SaveCustomer))
                    .Row(Row.Buttons)
            }
        };
    }

    public Guid? CustomerId => BindingContext.CustomerId;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCustomerCommand.Execute(CustomerId);
    }

    private enum Row
    {
        Form,
        Buttons
    }
}