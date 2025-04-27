using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using Microsoft.Maui.Controls; // <-- here
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages
{
    public sealed class CustomersPage : BasePage<CustomersViewModel>
    {
        private readonly CustomersViewModel _vm;

        public CustomersPage(CustomersViewModel vm) : base(vm)
        {
            _vm = vm;
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
                RowDefinitions = Rows.Define((Row.Header, Auto), (Row.Body, Star)),
                Children =
                {
                    BuildHeader(vm).Row(Row.Header),
                    BuildBody(vm).Row(Row.Body),
                    BuildBusyIndicator(vm).Row(Row.Body)
                }
            };

            // Loaded += async (_, _) => await vm.LoadCustomersCommand.ExecuteAsync(null);
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
                            ItemTemplate = new DataTemplate(() => BuildItemTemplate(vm))
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

        private static View BuildItemTemplate(CustomersViewModel vm)
        {
            // Wrap each record in a Frame keyed by CustomerId for UI testing
            return new Frame
                {
                    Margin = new Thickness(0, 5),
                    Padding = new Thickness(10),
                    HasShadow = true,
                    Content = new Grid
                    {
                        ColumnDefinitions = Columns.Define(
                            (CustCol.Info, Star),
                            (CustCol.Edit, Auto),
                            (CustCol.Schedule, Auto)),
                        Children =
                        {
                            // Customer info
                            new VerticalStackLayout
                                {
                                    Spacing = 4,
                                    Children =
                                    {
                                        new Label().Font(size: 16, bold: true)
                                            .Bind(Label.TextProperty, nameof(CustomerDto.FirstName)),
                                        new Label().Font(size: 16, bold: true)
                                            .Bind(Label.TextProperty, nameof(CustomerDto.LastName)),
                                        new Label().FontSize(14)
                                            .TextColor(Colors.Gray)
                                            .Bind(Label.TextProperty, nameof(CustomerDto.Email)),
                                        new Label().FontSize(14)
                                            .TextColor(Colors.Gray)
                                            .Bind(Label.TextProperty, nameof(CustomerDto.PhoneNumber))
                                    }
                                }
                                .Column(CustCol.Info),

                            // Edit button
                            new Button { Text = "Edit" }
                                .Bind(Button.CommandProperty, nameof(vm.EditCustomerCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, ".", source: vm)
                                .Margin(5, 0)
                                .Column(CustCol.Edit),

                            // Schedule Job button
                            new Button { Text = "Schedule Job" }
                                .Bind(Button.CommandProperty, nameof(vm.ScheduleJobCommand), source: vm)
                                .Bind(Button.CommandParameterProperty, ".", source: vm)
                                .Margin(5, 0)
                                .Column(CustCol.Schedule)
                        }
                    }
                }
                // Bind AutomationId to CustomerId for testing
                .Bind(AutomationIdProperty, nameof(CustomerDto.Id));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // kick off the load
            _vm.LoadCustomersCommand.Execute(null);
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

        private enum CustCol
        {
            Info,
            Edit,
            Schedule
        }
    }
}