﻿﻿using System;
using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

/// <summary>
/// Page for viewing a scheduled job, its occurrences, and reminders, with paging support.
/// </summary>
[QueryProperty(nameof(ScheduledJobDefinitionId), "ScheduledJobDefinitionId")]
[QueryProperty(nameof(CustomerId), "CustomerId")]
public sealed class ScheduledJobViewPage : BasePage<ScheduledJobViewModel>
{
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    public ScheduledJobViewPage(ScheduledJobViewModel vm) : base(vm)
    {
        Title = "Scheduled Job Details";
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    // Job Details Card
                    CreateJobDetailsCard(),

                    // Add Occurrence Button
                    CardStyles.CreatePrimaryButton("➕ Add Occurrence")
                        .BindCommand(nameof(ScheduledJobViewModel.AddOccurrenceCommand), source: vm),

                    // Job Occurrences Section
                    CreateOccurrencesSection(vm)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadScheduledJobCommand.Execute(
            new Details(
                new CustomerId(Guid.Parse(CustomerId)),
                new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))
            ));
    }

    private Frame CreateJobDetailsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Job Title
                CardStyles.CreateTitleLabel()
                    .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.Title)),

                // Customer info with icon
                CardStyles.CreateIconTextStack("👤",
                    CardStyles.CreateSubtitleLabel()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.CustomerName))),

                // Anchor date with icon
                CardStyles.CreateIconTextStack("📅",
                    CardStyles.CreateSubtitleLabel()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.AnchorDate),
                            stringFormat: "{0:MMM d, yyyy h:mm tt}")),

                // Description
                CardStyles.CreateIconTextStack("📝",
                    CardStyles.CreateSubtitleLabel()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.Description))),

                // Cron expression
                CardStyles.CreateIconTextStack("⚙️",
                    CardStyles.CreateCaptionLabel()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.CronExpression)))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private VerticalStackLayout CreateOccurrencesSection(ScheduledJobViewModel vm)
    {
        return new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                // Section header
                CardStyles.CreateTitleLabel()
                    .Text("Job Occurrences"),

                // Collection view
                new CollectionView
                {
                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                    {
                        ItemSpacing = 12
                    },
                    SelectionMode = SelectionMode.None,
                    EmptyView = CreateEmptyOccurrencesView(),
                    ItemTemplate = new DataTemplate(() => CreateOccurrenceCard(vm))
                }
                .Bind(ItemsView.ItemsSourceProperty, nameof(ScheduledJobViewModel.JobOccurrences)),

                // Load more button
                CardStyles.CreateSecondaryButton("Load More Occurrences")
                    .Bind(IsVisibleProperty, nameof(ScheduledJobViewModel.HasMoreOccurrences))
                    .Bind(Button.CommandProperty, nameof(ScheduledJobViewModel.LoadMoreOccurrencesCommand))
            }
        };
    }

    private static Frame CreateOccurrenceCard(ScheduledJobViewModel vm)
    {
        var content = new Grid
        {
            ColumnDefinitions = Columns.Define(
                (Column.Content, Star),
                (Column.Action, Auto)
            ),
            Children =
            {
                // Left side: Occurrence info
                new VerticalStackLayout
                {
                    Spacing = 6,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        // Occurrence date
                        CardStyles.CreateIconTextStack("📅",
                            CardStyles.CreateSubtitleLabel()
                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.OccurrenceDate),
                                    stringFormat: "{0:MMM d, yyyy h:mm tt}")),

                        // Job title
                        CardStyles.CreateIconTextStack("📋",
                            CardStyles.CreateCaptionLabel()
                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.JobTitle))),

                        // Completion status
                        CardStyles.CreateIconTextStack("✅",
                            CardStyles.CreateCaptionLabel()
                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.MarkedAsCompleted),
                                    stringFormat: "Completed: {0}"))
                    }
                }
                .Column(Column.Content),

                // Right side: View button
                CardStyles.CreateSecondaryButton("View")
                    .Bind(Button.CommandProperty, nameof(ScheduledJobViewModel.NavigateToOccurrenceCommand), source: vm)
                    .Bind(Button.CommandParameterProperty, nameof(JobOccurrenceDto.JobOccurrenceId))
                    .Column(Column.Action)
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private static VerticalStackLayout CreateEmptyOccurrencesView() =>
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
                    Text = "No Occurrences Yet",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardStyles.Colors.TextPrimary,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Job occurrences will appear here when they are created.",
                    FontSize = 16,
                    TextColor = CardStyles.Colors.TextSecondary,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

    private static VerticalStackLayout Section(string label, View control) =>
        new()
        {
            Spacing = 4,
            Children =
            {
                new Label().Text(label).Font(size: 14, bold: true),
                control
            }
        };

    private enum Column
    {
        Content,
        Action
    }
}