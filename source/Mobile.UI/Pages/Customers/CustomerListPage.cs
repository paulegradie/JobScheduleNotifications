using CommunityToolkit.Maui.Markup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers;

public sealed class CustomerListPage : BasePage<CustomerListModel>
{
    public CustomerListPage(CustomerListModel vm) : base(vm)
    {
        Title = "Customers";

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Add",
            IconImageSource = "plus.png",
            Command = ViewModel.AddCustomerCommand
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Home",
            IconImageSource = "home.png",
            Command = ViewModel.NavigateHomeCommand
        });

        Content = new Grid
        {
            Padding = 20,
            RowDefinitions = Rows.Define((Row.Header, Auto), (Row.Body, Star)),
            Children =
            {
                BuildHeader(ViewModel).Row(Row.Header),
                BuildBody(ViewModel).Row(Row.Body),
                BuildBusyIndicator(ViewModel).Row(Row.Body)
            }
        };

        /* first load */
        Loaded += async (_, _) => await ViewModel.LoadCustomersCommand.ExecuteAsync(null);
    }

    private static Grid BuildHeader(CustomerListModel vm) =>
        new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions = Columns.Define((Col.Search, Star), (Col.Add, Auto)),
            Children =
            {
                new SearchBar()
                    .Placeholder("Search customers…")
                    .TextColor(Colors.DarkGray)
                    .Bind(SearchBar.TextProperty, nameof(vm.SearchText), BindingMode.TwoWay)
                    .Bind(SearchBar.SearchCommandProperty, nameof(vm.LoadCustomersCommand))
                    .Column(Col.Search),

                new Button()
                    .Text("Add")
                    .Bind(Button.CommandProperty, nameof(vm.AddCustomerCommand))
                    .Column(Col.Add)
            }
        };

    private static RefreshView BuildBody(CustomerListModel vm) =>
        new RefreshView
            {
                Content = new CollectionView
                    {
                        SelectionMode = SelectionMode.None,
                        EmptyView = "No customers found",
                        Margin = new Thickness(0, 10, 0, 10),
                        ItemTemplate = new DataTemplate(() => BuildCustomerTemplate(vm))
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(vm.Customers))
            }
            .Bind(RefreshView.IsRefreshingProperty, nameof(vm.IsLoading))
            .Bind(RefreshView.CommandProperty, nameof(vm.LoadCustomersCommand));

    private static ActivityIndicator BuildBusyIndicator(CustomerListModel vm) =>
        new ActivityIndicator()
            .Center()
            .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsLoading))
            .Bind(IsVisibleProperty, nameof(vm.IsLoading));

    private static Frame BuildCustomerTemplate(CustomerListModel vm)
    {
        // Card-like container
        return new Frame
        {
            Padding = 40,
            Margin = 40,
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Margin = new Thickness(0, 10, 0, 10),
                Children =
                {
                    new HorizontalStackLayout()
                    {
                        Spacing = 5,
                        Children =
                        {
                            new Label().Font(size: 16, bold: true)
                                .Bind(Label.TextProperty, nameof(CustomerDto.FirstName)),
                            new Label().Font(size: 16, bold: true)
                                .Bind(Label.TextProperty, nameof(CustomerDto.LastName)),
                        }
                    },
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