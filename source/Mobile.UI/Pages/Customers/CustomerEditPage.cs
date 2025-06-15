
// CustomerEditPage.cs

using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages.Customers;

public sealed class CustomerEditPage : BasePage<CustomerEditModel>, IQueryAttributable
{
    private readonly CustomerEditModel _vm;

    public CustomerEditPage(CustomerEditModel vm) : base(vm)
    {
        _vm = vm;
        Title = vm.Title;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    new Label().Text("First Name").Font(size:14, bold:true),
                    new Entry()
                        .Placeholder("First Name")
                        .Bind(Entry.TextProperty, nameof(vm.FirstName)),

                    new Label().Text("Last Name").Font(size:14, bold:true),
                    new Entry()
                        .Placeholder("Last Name")
                        .Bind(Entry.TextProperty, nameof(vm.LastName)),

                    new Label().Text("Email").Font(size:14, bold:true),
                    new Entry()
                        .Placeholder("Email")
                        .Bind(Entry.TextProperty, nameof(vm.Email)),

                    new Label().Text("Phone").Font(size:14, bold:true),
                    new Entry()
                        .Placeholder("Phone")
                        .Bind(Entry.TextProperty, nameof(vm.PhoneNumber)),

                    new Label().Text("Notes").Font(size:14, bold:true),
                    new Editor { HeightRequest = 100 }
                        .Bind(Editor.TextProperty, nameof(vm.Notes)),

                    new Label()
                        .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                        .TextColor(Colors.Red)
                        .Bind(IsVisibleProperty, nameof(vm.ErrorMessage)),

                    new HorizontalStackLayout
                    {
                        Spacing = 15,
                        Children =
                        {
                            new Button { Text = "Save" }
                                .BindCommand(nameof(vm.SaveCustomerCommand)),
                                // .Bind(IsEnabledProperty, nameof(vm.CanSave)),
                            new Button { Text = "Cancel" }
                                .BindCommand(nameof(vm.CancelCommand))
                        }
                    }
                }
            }
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CustomerId", out var raw)
            && raw is string sid
            && Guid.TryParse(sid, out var id))
        {
            _vm.Initialize(id);
            Title = _vm.Title;
        }
        else
        {
            _vm.Initialize(null);
            Title = _vm.Title;
        }
    }
    
    private enum Row { Details, Buttons }
}
