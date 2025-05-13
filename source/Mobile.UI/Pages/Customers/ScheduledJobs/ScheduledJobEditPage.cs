using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
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
                            .Placeholder("Job title")
                            .Bind(Entry.TextProperty, nameof(ViewModel.Title))),

                    Section("Description",
                        new Editor { HeightRequest = 100 }
                            .Bind(Editor.TextProperty, nameof(ViewModel.Description))),

                    Section("Anchor Date",
                        new DatePicker()
                            .Bind(DatePicker.DateProperty, nameof(ViewModel.AnchorDate))),

                    Section("Frequency",
                        new Picker { ItemsSource = Enum.GetValues<Frequency>() }
                            .Bind(Picker.SelectedItemProperty,
                                  nameof(ViewModel.Frequency),
                                  mode: BindingMode.TwoWay)),

                    Section("Interval",
                        new Stepper { Minimum = 1, Maximum = 30 }
                            .Bind(Stepper.ValueProperty, nameof(ViewModel.Interval))),

                    Section("Day of Month",
                        new Entry { Keyboard = Keyboard.Numeric }
                            .Bind(Entry.TextProperty, nameof(ViewModel.DayOfMonth))),

                    Section("Cron Expression",
                        new Entry()
                            .Placeholder("Optional cron")
                            .Bind(Entry.TextProperty, nameof(ViewModel.CronExpression))),

                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.ErrorMessage))
                        .TextColor(Colors.Red)
                        .Bind(IsVisibleProperty, nameof(ViewModel.HasError)),

                    new Button()
                        .Text("Save")
                        .BindCommand(nameof(ViewModel.SaveCommand)),

                    new ActivityIndicator()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(ViewModel.IsBusy))
                }
            }
        };

        // Kick off the load as soon as the page appears:
        Loaded += async (_, _) =>
            await ViewModel.LoadForEditCommand.ExecuteAsync(
                new Details(
                    new CustomerId(Guid.Parse(CustomerId)),
                    new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))
                )
            );
    }

    static View Section(string label, View control) =>
        new VerticalStackLayout
        {
            Spacing = 2,
            Children =
            {
                new Label().Text(label).Font(size: 14, bold: true),
                control
            }
        };
}