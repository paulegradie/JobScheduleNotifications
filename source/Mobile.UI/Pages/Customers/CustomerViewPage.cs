using System;
using System.Collections.Generic;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers;

public sealed class CustomerViewPage : BasePage<CustomerViewModel>, IQueryAttributable
{
    private readonly CustomerViewModel _vm;

    public CustomerViewPage(CustomerViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = vm.Title;

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

                // Edit button
                new Button { Text = "Edit" }
                    .BindCommand(nameof(vm.EditCustomerCommand))
                    .Row(Row.Buttons)
            }
        };
    }

    // Receive customerId via Shell query
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CustomerId", out var raw)
            && raw is string sid
            && Guid.TryParse(sid, out var id))
        {
            _vm.LoadCustomerCommand.Execute(id);
        }
    }

    private enum Row
    {
        Details,
        Buttons
    }
}