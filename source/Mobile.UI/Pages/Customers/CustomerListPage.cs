﻿using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
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
            Padding = new Thickness(16),
            BackgroundColor = CardStyles.Colors.Background,
            RowDefinitions = Rows.Define((Row.Header, Auto), (Row.Body, Star)),
            Children =
            {
                BuildHeader(ViewModel).Row(Row.Header),
                BuildBody(ViewModel).Row(Row.Body),
                BuildBusyIndicator(ViewModel).Row(Row.Body)
            }
        };

        Loaded += async (_, _) => await ViewModel.LoadCustomersCommand.ExecuteAsync(null);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refresh the customer list when returning to this page
        ViewModel.LoadCustomersCommand.Execute(null);
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
                        EmptyView = CreateEmptyView(),
                        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                        {
                            ItemSpacing = 12
                        },
                        ItemTemplate = CreateCustomerCardTemplate(vm)
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

    private static DataTemplate CreateCustomerCardTemplate(CustomerListModel vm) =>
        new DataTemplate(() => CreateCustomerCard(vm));

    private static Frame CreateCustomerCard(CustomerListModel viewModel) =>
        CardStyles.CreateCard(
            new Grid
            {
                ColumnDefinitions = Columns.Define(
                    (Column.Content, Star),
                    (Column.Actions, Auto)
                ),
                Children =
                {
                    // Left side: Customer info
                    new VerticalStackLayout
                        {
                            Spacing = CardStyles.Spacing.ItemSpacing,
                            VerticalOptions = LayoutOptions.Center,
                            Children =
                            {
                                // Customer name
                                new HorizontalStackLayout
                                {
                                    Spacing = 5,
                                    Children =
                                    {
                                        CardStyles.CreateTitleLabel()
                                            .Bind(Label.TextProperty, nameof(CustomerDto.FirstName)),
                                        CardStyles.CreateTitleLabel()
                                            .Bind(Label.TextProperty, nameof(CustomerDto.LastName))
                                    }
                                },

                                // Email with icon
                                CardStyles.CreateIconTextStack("📧",
                                    CardStyles.CreateSubtitleLabel()
                                        .Bind(Label.TextProperty, nameof(CustomerDto.Email))),

                                // Phone with icon
                                CardStyles.CreateIconTextStack("📱",
                                    CardStyles.CreateSubtitleLabel()
                                        .Bind(Label.TextProperty, nameof(CustomerDto.PhoneNumber)))
                            }
                        }
                        .Column(Column.Content),

                    // Right side: Action buttons
                    new VerticalStackLayout
                        {
                            Spacing = 6,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.End,
                            Children =
                            {
                                // Primary action: View Jobs
                                CardStyles.CreatePrimaryButton("View Jobs")
                                    .Bind(Button.CommandProperty, nameof(viewModel.ViewJobsCommand), source: viewModel)
                                    .Bind(Button.CommandParameterProperty, "."),

                                // Secondary actions in a row
                                new HorizontalStackLayout
                                {
                                    Spacing = 6,
                                    Children =
                                    {
                                        CardStyles.CreateSecondaryButton("Edit")
                                            .Bind(Button.CommandProperty, nameof(viewModel.EditCustomerCommand), source: viewModel)
                                            .Bind(Button.CommandParameterProperty, "."),

                                        // CardStyles.CreateSecondaryButton("New Job", CardStyles.Colors.Success)
                                        //     .Bind(Button.CommandProperty, nameof(viewModel.CreateJobCommand), source: viewModel)
                                        //     .Bind(Button.CommandParameterProperty, "."),

                                        CardStyles.CreateSecondaryButton("Delete", CardStyles.Colors.Error)
                                            .Bind(Button.CommandProperty, nameof(viewModel.DeleteCustomerCommand), source: viewModel)
                                            .Bind(Button.CommandParameterProperty, ".")
                                    }
                                }
                            }
                        }
                        .Column(Column.Actions)
                }
            },
            CardStyles.Colors.Primary // Blue accent bar
        );

    private static VerticalStackLayout CreateEmptyView() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(40),
            Children =
            {
                new Label
                {
                    Text = "👥",
                    FontSize = 48,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "No Customers",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardStyles.Colors.TextPrimary,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Add your first customer to get started.",
                    FontSize = 16,
                    TextColor = CardStyles.Colors.TextSecondary,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

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

    private enum Column
    {
        Content,
        Actions
    }
}