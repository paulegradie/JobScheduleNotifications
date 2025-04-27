using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages;

public sealed class CreateCustomerPage : BasePage<CreateCustomerViewModel>
{
    private readonly CreateCustomerViewModel _vm;

    public CreateCustomerPage(CreateCustomerViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = "Add Customer";

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
                        Spacing = 10,
                        Children =
                        {
                            new Button { Text = "Save" }
                                .BindCommand(nameof(vm.SaveCommand))
                                .Bind(IsEnabledProperty, nameof(vm.CanSave)),
                            new Button { Text = "Cancel" }
                                .BindCommand(nameof(vm.CancelCommand))
                        }
                    }
                }
            }
        };
    }
}