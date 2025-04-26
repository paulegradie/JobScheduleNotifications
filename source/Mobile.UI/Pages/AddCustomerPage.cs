using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class AddCustomerPage : BasePage<CustomerViewModel>
{
    readonly CustomerViewModel _vm;

    public AddCustomerPage(CustomerViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = "Add Customer";

        // ScrollView in case notes grow
        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = 20,
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.FirstName, Auto),
                    (Row.LastName, Auto),
                    (Row.Email, Auto),
                    (Row.Phone, Auto),
                    (Row.Notes, Star),
                    (Row.Buttons, Auto)
                ),
                ColumnDefinitions = Columns.Define(
                    (Col.Label, Auto),
                    (Col.Input, Star)
                ),
                Children =
                {
                    // First Name
                    new Label().Text("First Name:").Font(size: 14, bold: true)
                        .Row(Row.FirstName).Column(Col.Label),
                    new Entry().Placeholder("Enter first name")
                        .Bind(Entry.TextProperty, nameof(vm.SelectedCustomer.FirstName))
                        .Row(Row.FirstName).Column(Col.Input),

                    // Last Name
                    new Label().Text("Last Name:").Font(size: 14, bold: true)
                        .Row(Row.LastName).Column(Col.Label),
                    new Entry().Placeholder("Enter last name")
                        .Bind(Entry.TextProperty, nameof(vm.SelectedCustomer.LastName))
                        .Row(Row.LastName).Column(Col.Input),

                    // Email
                    new Label().Text("Email:").Font(size: 14, bold: true)
                        .Row(Row.Email).Column(Col.Label),
                    new Entry { Keyboard = Keyboard.Email }
                        .Placeholder("Enter email")
                        .Bind(Entry.TextProperty, nameof(vm.SelectedCustomer.Email))
                        .Row(Row.Email).Column(Col.Input),

                    // Phone
                    new Label().Text("Phone:").Font(size: 14, bold: true)
                        .Row(Row.Phone).Column(Col.Label),
                    new Entry { Keyboard = Keyboard.Telephone }
                        .Placeholder("Enter phone number")
                        .Bind(Entry.TextProperty, nameof(vm.SelectedCustomer.PhoneNumber))
                        .Row(Row.Phone).Column(Col.Input),

                    // Notes
                    new Label().Text("Notes:").Font(size: 14, bold: true)
                        .Row(Row.Notes).Column(Col.Label),
                    new Editor()
                        .Placeholder("Additional notes…")
                        .Bind(Editor.TextProperty, nameof(vm.SelectedCustomer.Notes))
                        .Row(Row.Notes).ColumnSpan(2),

                    // Buttons
                    new HorizontalStackLayout
                        {
                            Spacing = 20,
                            Children =
                            {
                                new Button().Text("Save")
                                    .BindCommand(nameof(vm.SaveCustomerCommand))
                                    .Bind(IsEnabledProperty, nameof(vm.CanSave)),
                                new Button().Text("Cancel")
                                    .BindCommand(nameof(vm.CancelCommand))
                            }
                        }
                        .Center()
                        .Row(Row.Buttons)
                        .ColumnSpan(2)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Initialize a new DTO if we're adding
        if (!_vm.IsEditing)
        {
            _vm.SelectedCustomer = new CustomerDto();
            _vm.Title = "Add Customer";
        }
    }

    enum Row
    {
        FirstName,
        LastName,
        Email,
        Phone,
        Notes,
        Buttons
    }

    enum Col
    {
        Label,
        Input
    }
}