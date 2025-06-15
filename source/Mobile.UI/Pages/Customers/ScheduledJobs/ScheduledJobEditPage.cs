using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public record LoadParametersCustomerIdAndScheduleJobDefId(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId);

[ScheduledJobDefinitionIdQueryParam]
[CustomerIdQueryParam]
public sealed class ScheduledJobEditPage : BasePage<ScheduledJobEditModel>
{
    public string CustomerId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }

    public ScheduledJobEditPage(ScheduledJobEditModel vm) : base(vm)
    {
        Title = "Edit Scheduled Job";
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
        await ViewModel.LoadForEditCommand.ExecuteAsync(
            new LoadParametersCustomerIdAndScheduleJobDefId(
                new CustomerId(Guid.Parse(CustomerId)),
                new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))
            )
        );
    }

    private Frame CreateBasicInfoCard(ScheduledJobEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📋 Job Information"),

                // Title entry with validation
                CreateValidatedField("Job Title", "Enter job title...",
                    nameof(vm.Title), nameof(vm.IsTitleValid), nameof(vm.TitleError), vm),

                // Description entry with validation
                CreateValidatedField("Description", "Enter job description...",
                    nameof(vm.Description), nameof(vm.IsDescriptionValid), nameof(vm.DescriptionError), vm)
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateScheduleCard(ScheduledJobEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("⏰ Schedule Configuration"),

                // Start date
                Section("Start Date",
                    CardStyles.CreateStyledDatePicker(nameof(vm.AnchorDate))
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

    private Frame CreateActionsCard(ScheduledJobEditModel vm)
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
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.HasError), source: vm),

                // Validation message
                new Label
                    {
                        TextColor = CardStyles.Colors.Warning,
                        FontSize = CardStyles.Typography.CaptionSize,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(vm.ValidationMessage), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.ValidationMessage), source: vm,
                        converter: new StringToBoolConverter()),

                // Button container
                new HorizontalStackLayout
                {
                    Spacing = 12,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        // Save button
                        CardStyles.CreatePrimaryButton("💾 Update Job")
                            .BindCommand(nameof(vm.SaveCommand), source: vm)
                            .Bind(IsEnabledProperty, nameof(vm.CanSave), source: vm),
                        // Cancel button
                        CardStyles.CreateSecondaryButton("❌ Cancel", CardStyles.Colors.TextSecondary)
                            .BindCommand(nameof(vm.GoBackCommand))
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
                nameof(ScheduledJobEditModel.SelectFrequencyCommand),
                parameterSource: freq
            )
            .Bind(Button.StyleProperty, nameof(ViewModel.ChipStyle), converterParameter: freq);

    private VerticalStackLayout CreateValidatedField(string label, string placeholder,
        string textBindingPath, string validationBindingPath, string errorBindingPath,
        ScheduledJobEditModel vm, Keyboard? keyboard = null)
    {
        var entry = new Entry
        {
            Placeholder = placeholder,
            TextColor = CardStyles.Colors.TextPrimary,
            Keyboard = keyboard ?? Keyboard.Default
        }
        .Bind(Entry.TextProperty, textBindingPath, source: vm);

        // Bind background color based on validation state
        entry.SetBinding(Entry.BackgroundColorProperty, new Binding(validationBindingPath,
            source: vm,
            converter: new ValidationToColorConverter()));

        var errorLabel = new Label
        {
            FontSize = CardStyles.Typography.CaptionSize,
            TextColor = CardStyles.Colors.Error
        }
        .Bind(Label.TextProperty, errorBindingPath, source: vm)
        .Bind(IsVisibleProperty, errorBindingPath, source: vm,
            converter: new StringToBoolConverter());

        return new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label
                {
                    Text = $"{label}:",
                    FontSize = CardStyles.Typography.CaptionSize,
                    TextColor = CardStyles.Colors.TextSecondary
                },
                entry,
                errorLabel
            }
        };
    }

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
}