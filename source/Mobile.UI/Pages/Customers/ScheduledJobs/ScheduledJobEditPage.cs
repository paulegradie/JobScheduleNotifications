using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public record Details(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId);

[QueryProperty(nameof(CustomerId), "customerId")]
[QueryProperty(nameof(ScheduledJobDefinitionId), "scheduledJobDefinitionId")]
public sealed class ScheduledJobEditPage : BasePage<ScheduledJobEditModel>
{
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadForEditCommand.Execute(new Details(
            new CustomerId(Guid.Parse(CustomerId)), new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))));
    }

    public ScheduledJobEditPage(ScheduledJobEditModel vm) : base(vm)
    {
        Title = "Edit a Scheduled Job:";
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
                        .Bind(IsVisibleProperty, nameof(ViewModel.ErrorMessage)),

                    new Button()
                        .Text("Save")
                        .BindCommand(nameof(ViewModel.SaveCommand))
                        .Bind(IsEnabledProperty, nameof(vm.IsBusy), converter: new InvertedBoolConverter()),

                    new ActivityIndicator()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                }
            }
        };
    }

    private static View Section(string label, View control) =>
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