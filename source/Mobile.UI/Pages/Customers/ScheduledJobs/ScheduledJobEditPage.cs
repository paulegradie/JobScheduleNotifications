using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public record Details(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId);

[QueryProperty(nameof(CustomerId), nameof(CustomerId))]
[QueryProperty(nameof(ScheduledJobDefinitionId), nameof(ScheduledJobDefinitionId))]
public sealed class ScheduledJobEditPage : BasePage<ScheduledJobEditModel>
{
    public string CustomerId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }

    public ScheduledJobEditPage(ScheduledJobEditModel vm) : base(vm)
    {
        Title = "Edit a Scheduled Job";
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    Section("Title",
                        new Entry()
                            .Placeholder("Job Title")
                            .Bind(Entry.TextProperty, nameof(vm.Title))
                    ),

                    Section("Description",
                        new Editor { HeightRequest = 100 }
                            .Bind(Editor.TextProperty, nameof(vm.Description))
                    ),

                    Section("Anchor Date",
                        new DatePicker()
                            .Bind(DatePicker.DateProperty, nameof(vm.AnchorDate))
                    ),
                    new Label().Text("Frequency").Font(size: 14, bold: true),
                    new FlexLayout
                    {
                        JustifyContent = FlexJustify.SpaceBetween,
                        Children =
                        {
                            CreateChip(Frequency.Daily),
                            CreateChip(Frequency.Weekly),
                            CreateChip(Frequency.Monthly),
                            CreateChip(Frequency.Custom)
                        }
                    },

                    new Label().Bind(Label.TextProperty, nameof(vm.IntervalDisplay)).Font(size: 18),
                    new Slider(1, 52, 1)
                        .Bind(Slider.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay),
                    new Stepper(1, 100, 1, 1)
                        .Bind(Stepper.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay),

                    Section("Cron Preview",
                        new Label { FontSize = 14, TextColor = Colors.Gray }
                            .Bind(Label.TextProperty, nameof(vm.CronPreview))
                    ),

                    new Label { TextColor = Colors.Red }
                        .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                        .Bind(IsVisibleProperty, nameof(vm.HasError)),

                    new Button { Text = "Save Job", CornerRadius = 8 }
                        .BindCommand(nameof(vm.SaveCommand))
                        .Bind(IsEnabledProperty, nameof(vm.CanSave)),

                    new ActivityIndicator()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                        .Bind(IsVisibleProperty, nameof(vm.IsBusy))
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        await ViewModel.LoadForEditCommand.ExecuteAsync(
            new Details(
                new CustomerId(Guid.Parse(CustomerId)),
                new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))
            )
        );
    }

    private Button CreateChip(Frequency freq) =>
        new Button
            {
                Text = freq.ToString(),
                CornerRadius = 20,
                Padding = new Thickness(16, 8)
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
}