﻿﻿﻿using CommunityToolkit.Maui.Markup;
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
            BorderColor = Colors.LightGray,
            CornerRadius = 8,
            HasShadow = false,
            Padding = 16,
            Margin = new Thickness(0, 4),
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    // Job title
                    new Label()
                        .Font(size: 18, bold: true)
                        .TextColor(Colors.Black)
                        .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.Title)),

                    // Job date
                    new Label()
                        .Font(size: 14)
                        .TextColor(Colors.Gray)
                        .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.AnchorDate),
                            stringFormat: "Date: {0:MMM d, yyyy}"),

                    // Simple button
                    new Button
                    {
                        Text = "View",
                        BackgroundColor = Colors.Blue,
                        TextColor = Colors.White,
                        CornerRadius = 4,
                        FontSize = 14
                    }
                    .Bind(Button.CommandProperty, nameof(ScheduledJobListModel.NavigateToViewCommand), source: ViewModel)
                    .Bind(Button.CommandParameterProperty, nameof(ScheduledJobDefinitionDto.ScheduledJobDefinitionId))
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