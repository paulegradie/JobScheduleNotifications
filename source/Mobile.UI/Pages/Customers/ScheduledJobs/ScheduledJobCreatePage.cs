﻿﻿﻿﻿﻿using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[CustomerIdQueryParam]
public class ScheduledJobCreatePage : BasePage<ScheduledJobCreateModel>
{
    public string CustomerId { get; set; }

    public ScheduledJobCreatePage(ScheduledJobCreateModel vm) : base(vm)
    {
        Title = "Create Scheduled Job";
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    // Basic Information Card
                    CreateBasicInfoCard(vm),

                    // Schedule Configuration Card
                    CreateScheduleCard(vm),

                    // Actions Card
                    CreateActionsCard(vm)
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Clear all fields when the page appears to ensure fresh form
        ViewModel.ClearFieldsCommand.Execute(null);
        // Load customers and set selected customer
        await ViewModel.LoadCommand.ExecuteAsync(CustomerId);
    }

    private Frame CreateBasicInfoCard(ScheduledJobCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📋 Basic Information"),

                // Customer picker
                Section("Customer",
                    new Picker
                        {
                            Title = "Select Customer",
                            ItemDisplayBinding = new Binding(nameof(CustomerDto.FullName)),
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Picker.ItemsSourceProperty, nameof(vm.Customers))
                        .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedCustomer))
                ),

                // Title entry
                Section("Job Title",
                    new Entry
                        {
                            Placeholder = "Enter job title...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.Title))
                ),

                // Description editor
                Section("Description",
                    new Editor
                        {
                            HeightRequest = 100,
                            Placeholder = "Enter job description...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Editor.TextProperty, nameof(vm.Description))
                )
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateScheduleCard(ScheduledJobCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("⏰ Schedule Configuration"),

                // Anchor date section
                Section("Start Date & Time",
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new DatePicker
                                {
                                    BackgroundColor = Colors.White,
                                    TextColor = CardStyles.Colors.TextPrimary,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                }
                                .Bind(DatePicker.DateProperty, nameof(vm.AnchorDate)),

                            new TimePicker
                                {
                                    BackgroundColor = Colors.White,
                                    TextColor = CardStyles.Colors.TextPrimary,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                }
                                .Bind(TimePicker.TimeProperty, nameof(vm.AnchorTime))
                                .Invoke(tp =>
                                {
                                    tp.TimeSelected += (s, e) =>
                                    {
                                        var min = new TimeSpan(6, 0, 0);
                                        var max = new TimeSpan(18, 0, 0);
                                        if (e.NewTime < min)
                                            tp.Time = min;
                                        else if (e.NewTime > max)
                                            tp.Time = max;
                                    };
                                })
                        }
                    }
                ),

                // Frequency chips
                Section("Frequency",
                    new FlexLayout
                    {
                        JustifyContent = FlexJustify.SpaceBetween,
                        Children =
                        {
                            CreateFrequencyChip(Frequency.Daily),
                            CreateFrequencyChip(Frequency.Weekly),
                            CreateFrequencyChip(Frequency.Monthly),
                        }
                    }
                ),

                // Interval display
                CardStyles.CreateSubtitleLabel()
                    .Bind(Label.TextProperty, nameof(vm.IntervalDisplay)),

                // Interval controls
                Section("Interval",
                    new VerticalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Slider(1, 52, 1)
                                {
                                    ThumbColor = CardStyles.Colors.Primary,
                                    MinimumTrackColor = CardStyles.Colors.Primary
                                }
                                .Bind(Slider.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay),

                            new Stepper(1, 100, 1, 1)
                                {
                                    BackgroundColor = Colors.White
                                }
                                .Bind(Stepper.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay)
                        }
                    }
                ),

                // Day of month (conditional)
                Section("Day of Month",
                    new Entry
                        {
                            Keyboard = Keyboard.Numeric,
                            Placeholder = "Enter day (1-31)",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.DayOfMonth))
                ).Bind(IsVisibleProperty, nameof(vm.ShowDayOfMonth)),

                // Cron preview
                Section("Cron Preview",
                    new Label
                        {
                            FontSize = CardStyles.Typography.CaptionSize,
                            TextColor = CardStyles.Colors.TextSecondary,
                            FontFamily = "Courier"
                        }
                        .Bind(Label.TextProperty, nameof(vm.CronPreview))
                )
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Warning);
    }

    private Frame CreateActionsCard(ScheduledJobCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Error message
                new Label
                    {
                        TextColor = CardStyles.Colors.Error,
                        FontSize = CardStyles.Typography.SubtitleSize
                    }
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                    .Bind(IsVisibleProperty, nameof(vm.HasError)),

                // Button container
                new Grid
                {
                    ColumnDefinitions = Columns.Define(
                        (Column.Save, Star),
                        (Column.Cancel, Star)
                    ),
                    ColumnSpacing = 12,
                    Children =
                    {
                        // Save button
                        CardStyles.CreatePrimaryButton("💾 Save Job")
                            .BindCommand(nameof(vm.SaveCommand), source: vm)
                            .Bind(IsEnabledProperty, nameof(vm.CanSave), source: vm)
                            .Column(Column.Save),

                        // Cancel button
                        CardStyles.CreateSecondaryButton("❌ Cancel")
                            .BindCommand(nameof(vm.CancelCommand), source: vm)
                            .Column(Column.Cancel)
                    }
                },

                // Loading indicator
                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                    .Bind(IsVisibleProperty, nameof(vm.IsBusy))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Button CreateFrequencyChip(Frequency freq) =>
        new Button
            {
                Text = freq.ToString(),
                CornerRadius = 20,
                Padding = new Thickness(16, 8),
                FontSize = CardStyles.Typography.CaptionSize
            }
            .BindCommand(
                nameof(ScheduledJobCreateModel.SelectFrequencyCommand),
                parameterSource: freq
            )
            .Bind(Button.StyleProperty, nameof(ViewModel.ChipStyle), converterParameter: freq);

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
        Save,
        Cancel
    }
}