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

        Content = new Grid
        {
            Padding = 20,
            RowDefinitions = GridRowsColumns.Rows.Define((Row.Header, Auto), (Row.Body, Star)),
            Children =
            {
                BuildHeader(vm).Row(Row.Header),
                BuildBody(vm).Row(Row.Body),
                BuildBusyIndicator(vm).Row(Row.Body)
            }
        };

        Loaded += async (_, _) => await vm.LoadCustomersCommand.ExecuteAsync(null);
    }

    private static Grid BuildHeader(CustomersViewModel vm) =>
        new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions = GridRowsColumns.Columns.Define((Col.Search, Star), (Col.Add, Auto)),
            Children =
            {
                new SearchBar()
                    .Placeholder("Search customers…")
                    .Bind(SearchBar.TextProperty, nameof(vm.SearchText), BindingMode.TwoWay)
                    .Bind(SearchBar.SearchCommandProperty, nameof(vm.LoadCustomersCommand))
                    .Column(Col.Search),

                new Button { Text = "Add" }
                    .Bind(Button.CommandProperty, nameof(vm.AddCustomerCommand))
                    .BackgroundColor(Colors.CadetBlue)
                    .TextColor(Colors.White)
                    .Size(80, 40)
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
                        ItemTemplate = new DataTemplate(() => BuildSwipeTemplate(vm))
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

    private static SwipeView BuildSwipeTemplate(CustomersViewModel vm)
    {
        var swipeItems = new SwipeItems
        {
            new SwipeItem
                {
                    Text = "Edit",
                    BackgroundColor = Colors.CadetBlue
                }
                .Bind(SwipeItem.CommandProperty, nameof(vm.EditCustomerCommand), source: vm)
                .Bind(SwipeItem.CommandParameterProperty, "."),

            new SwipeItem
                {
                    Text = "Delete",
                    BackgroundColor = Colors.IndianRed
                }
                .Bind(SwipeItem.CommandProperty, nameof(vm.DeleteCustomerCommand), source: vm)
                .Bind(SwipeItem.CommandParameterProperty, ".")
        };

        return new SwipeView
        {
            RightItems = swipeItems,
            Content = BuildRowContent()
        };
    }

    private static Grid BuildRowContent() =>
        new Grid
        {
            Padding = 10,
            ColumnDefinitions = Columns.Define((CustCol.Info, Star), (CustCol.Chevron, Auto)),
            Children =
            {
                new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label().Font(size:16, bold:true)
                            .Bind(Label.TextProperty, nameof(CustomerDto.FirstName)),
                        new Label().Font(size:16, bold:true)
                            .Bind(Label.TextProperty, nameof(CustomerDto.LastName)),
                        new Label().FontSize(14)
                            .TextColor(Colors.Gray)
                            .Bind(Label.TextProperty, nameof(CustomerDto.Email)),
                        new Label().FontSize(14)
                            .TextColor(Colors.Gray)
                            .Bind(Label.TextProperty, nameof(CustomerDto.PhoneNumber))
                    }
                }.Column(CustCol.Info),

                new Image
                {
                    Source = "chevron_right.png",
                    HeightRequest = 20,
                    WidthRequest = 20,
                    VerticalOptions = LayoutOptions.Center
                }.Column(CustCol.Chevron)
            }
        };

    private enum Row { Header, Body }
    private enum Col { Search, Add }
    private enum CustCol { Info, Chevron }
}