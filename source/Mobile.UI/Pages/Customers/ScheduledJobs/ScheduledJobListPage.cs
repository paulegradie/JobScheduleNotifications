﻿﻿using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(CustomerId), "CustomerId")]
public sealed class ScheduledJobListPage : BasePage<ScheduledJobListModel>
{
    public string CustomerId { get; set; }

    public ScheduledJobListPage(ScheduledJobListModel vm) : base(vm)
    {
        Title = "Scheduled Jobs";

        Content = new Grid
        {
            Padding = new Thickness(16),
            BackgroundColor = Color.FromArgb("#F8F9FA"), // Light background
            RowDefinitions = Rows.Define((Row.MainContent, Star)),
            Children =
            {
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
                                ItemTemplate = CreateJobCardTemplate()
                            }
                            .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.ScheduledJobs))
                    }
                    .Bind(RefreshView.IsRefreshingProperty, nameof(ViewModel.IsBusy))
                    .Row(Row.MainContent)
            }
        };
    }

    private DataTemplate CreateJobCardTemplate() =>
        new DataTemplate(() => CreateJobCard());

    private Frame CreateJobCard() =>
        new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = Color.FromArgb("#E1E5E9"),
            CornerRadius = 12,
            HasShadow = false,
            Padding = 0,
            Margin = new Thickness(0, 6),
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    // Blue accent bar at top
                    new BoxView
                    {
                        BackgroundColor = Color.FromArgb("#2196F3"), // Primary500
                        HeightRequest = 4
                    },

                    // Main content with side-by-side layout
                    new Grid
                    {
                        Padding = new Thickness(20, 16),
                        ColumnDefinitions = Columns.Define(
                            (Column.Content, Star),
                            (Column.Action, Auto)
                        ),
                        Children =
                        {
                            // Left side: Job info
                            new VerticalStackLayout
                            {
                                Spacing = 8,
                                VerticalOptions = LayoutOptions.Center,
                                Children =
                                {
                                    // Job title
                                    new Label()
                                        .Font(size: 18, bold: true)
                                        .TextColor(Color.FromArgb("#212121")) // TextPrimary
                                        .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.Title)),

                                    // Date with icon
                                    new HorizontalStackLayout
                                    {
                                        Spacing = 6,
                                        Children =
                                        {
                                            new Label()
                                                .Text("📅")
                                                .FontSize(14),
                                            new Label()
                                                .Font(size: 14)
                                                .TextColor(Color.FromArgb("#757575")) // TextSecondary
                                                .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.AnchorDate),
                                                    stringFormat: "{0:MMM d, yyyy}")
                                        }
                                    },

                                    // Customer info with icon
                                    new HorizontalStackLayout
                                    {
                                        Spacing = 6,
                                        Children =
                                        {
                                            new Label()
                                                .Text("👤")
                                                .FontSize(14),
                                            new Label()
                                                .Font(size: 13)
                                                .TextColor(Color.FromArgb("#757575")) // TextSecondary
                                                .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.CustomerId),
                                                    stringFormat: "Customer: {0}")
                                        }
                                    }
                                }
                            }
                            .Column(Column.Content),

                            // Right side: Action button
                            new Button
                            {
                                Text = "View Details",
                                BackgroundColor = Color.FromArgb("#2196F3"), // Primary500
                                TextColor = Colors.White,
                                CornerRadius = 8,
                                FontSize = 14,
                                Padding = new Thickness(16, 10),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.End
                            }
                            .Bind(Button.CommandProperty, nameof(ScheduledJobListModel.NavigateToViewCommand), source: ViewModel)
                            .Bind(Button.CommandParameterProperty, nameof(ScheduledJobDefinitionDto.ScheduledJobDefinitionId))
                            .Column(Column.Action)
                        }
                    }
                }
            }
        };

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
                    Text = "📋",
                    FontSize = 48,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "No Scheduled Jobs",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#212121"), // TextPrimary
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "There are no scheduled jobs for this customer yet.",
                    FontSize = 16,
                    TextColor = Color.FromArgb("#757575"), // TextSecondary
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };


    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadForCustomerCommand.Execute(CustomerId);
    }

    private enum Row
    {
        MainContent,
        CardTitle,
        CardDetails
    }

    private enum Column
    {
        Content,
        Action
    }
}