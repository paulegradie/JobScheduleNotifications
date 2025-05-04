using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(CustomerId), "customerId")]
public partial class ScheduledJobCreatePage : BasePage<ScheduledJobCreateModel>
{
    public string CustomerId { get; set; }

    public ScheduledJobCreatePage(ScheduledJobCreateModel vm) : base(vm)
    {
        Title = "Create a Scheduled Job for this Customer:";
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    Section("Customer",
                        new Picker()
                            {
                                Title = "Where does this go",
                                ItemDisplayBinding = new Binding(nameof(CustomerDto.FullName)),
                            }
                            .TextColor(Colors.White)
                            .Bind(Picker.ItemsSourceProperty, nameof(vm.Customers))
                            .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedCustomer))),

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
                            .Bind(Picker.SelectedItemProperty, nameof(ViewModel.Frequency))),

                    Section("Interval",
                        new Stepper { Minimum = 1, Maximum = 30 }
                            .Bind(Stepper.ValueProperty, nameof(ViewModel.Interval))),

                    Section("Week Days",
                        new HorizontalStackLayout
                        {
                            Spacing = 5,
                            // you can generate CheckBoxes here bound to ViewModel.SelectedWeekDays
                        }),

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
                        .Text("Create Job")
                        .BindCommand(nameof(ViewModel.CreateJobCommand))
                        .Bind(IsEnabledProperty, nameof(ViewModel.IsBusy),
                            converter: new InvertedBoolConverter()),

                    new ActivityIndicator()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(ViewModel.IsBusy)),
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadCustomersCommand.Execute(CustomerId);
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