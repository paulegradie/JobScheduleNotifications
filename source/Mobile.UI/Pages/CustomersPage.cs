using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class CustomersPage : BasePage<CustomersViewModel>
{
    public CustomersPage(CustomersViewModel vm) : base(vm)
    {
        Title = "Customers";

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Add",
            IconImageSource = "plus.png",
            Command = vm.AddCustomerCommand
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Home",
            IconImageSource = "home.png",
            // Command = vm.NavigateHomeCommand
        });

        Content = new Grid
        {
            Padding = 20,
            RowDefinitions = Rows.Define((Row.Header, Auto), (Row.Body, Star)),
            Children =
            {
                BuildHeader(vm).Row(Row.Header),
                BuildBody(vm).Row(Row.Body),
                BuildBusyIndicator(vm).Row(Row.Body)
            }
        };

        /* first load */
        Loaded += async (_, _) => await vm.LoadCustomersCommand.ExecuteAsync(null);
    }

    private static Grid BuildHeader(CustomersViewModel vm) =>
        new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions = Columns.Define((Col.Search, Star), (Col.Add, Auto)),
            Children =
            {
                new SearchBar()
                    .Placeholder("Search customers…")
                    .Bind(SearchBar.TextProperty, nameof(vm.SearchText), BindingMode.TwoWay)
                    .Bind(SearchBar.SearchCommandProperty, nameof(vm.LoadCustomersCommand))
                    .Column(Col.Search),

                new Button()
                    .Text("Add")
                    .Bind(Button.CommandProperty, nameof(vm.AddCustomerCommand))
                    .Column(Col.Add)
            }
        };

    private static RefreshView BuildBody(CustomersViewModel vm) =>
        new RefreshView
            {
                Content = new CollectionView
                    {
                        SelectionMode = SelectionMode.None,
                        EmptyView = "No customers found",
                        ItemTemplate = new DataTemplate(() => BuildCustomerTemplate(vm))
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(vm.Customers))
            }
            .Bind(RefreshView.IsRefreshingProperty, nameof(vm.IsLoading))
            .Bind(RefreshView.CommandProperty, nameof(vm.LoadCustomersCommand));

    private static ActivityIndicator BuildBusyIndicator(CustomersViewModel vm) =>
        new ActivityIndicator()
            .Center()
            .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsLoading))
            .Bind(IsVisibleProperty, nameof(vm.IsLoading));

    private static Frame BuildCustomerTemplate(CustomersViewModel vm)
    {
        // Card-like container
        return new Frame
        {
            Padding = 10,
            Margin = new Thickness(0, 0, 0, 10),
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label().Font(size: 16, bold: true)
                        .Bind(Label.TextProperty, nameof(CustomerDto.FirstName)),
                    new Label().Font(size: 16, bold: true)
                        .Bind(Label.TextProperty, nameof(CustomerDto.LastName)),
                    new Label().FontSize(14)
                        .TextColor((Color)Application.Current.Resources["TextSecondary"])
                        .Bind(Label.TextProperty, nameof(CustomerDto.Email)),
                    new Label().FontSize(14)
                        .TextColor((Color)Application.Current.Resources["TextSecondary"])
                        .Bind(Label.TextProperty, nameof(CustomerDto.PhoneNumber)),

                    // Buttons row
                    new HorizontalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Button { Text = "Edit" }
                                .Bind(Button.CommandProperty, nameof(vm.EditCustomerCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, "."),
                            new Button { Text = "Delete" }
                                .Bind(Button.CommandProperty, nameof(vm.DeleteCustomerCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, "."),
                            new Button { Text = "New Job" }
                                .Bind(Button.CommandProperty, nameof(vm.CreateJobCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, "."),
                            new Button { Text = "View Jobs" }
                                .Bind(Button.CommandProperty, nameof(vm.ViewJobsCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, ".")
                        }
                    }
                }
            }
        };
    }

    private enum Row
    {
        Header,
        Body
    }

    private enum Col
    {
        Search,
        Add
    }
}