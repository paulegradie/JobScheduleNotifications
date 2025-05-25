using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(CustomerId), nameof(CustomerId))]
public class ScheduledJobCreatePage : BasePage<ScheduledJobCreateModel>
{
    public string CustomerId { get; set; }

    public ScheduledJobCreatePage(ScheduledJobCreateModel vm) : base(vm)
    {
        Title = "Create A Scheduled Job";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 25,
                Children =
                {
                    Section("Customer",
                        new Picker { Title = "Select Customer", ItemDisplayBinding = new Binding(nameof(CustomerDto.FullName)) }
                            .Bind(Picker.ItemsSourceProperty, nameof(vm.Customers))
                            .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedCustomer))
                    ),

                    Section("Title",
                        new Entry()
                            .Placeholder("Job Title")
                            .Bind(Entry.TextProperty, nameof(vm.Title))
                    ),

                    Section("Description",
                        new Editor { HeightRequest = 100 }
                            .Bind(Editor.TextProperty, nameof(vm.Description))
                    ),
                    new Label().Text("Anchor Date").Font(size: 14, bold: true),
                    new HorizontalStackLayout()
                    {
                        Spacing = 4,
                        Children =
                        {
                            new DatePicker().Bind(DatePicker.DateProperty, nameof(vm.AnchorDate)),
                            new TimePicker()
                                .Bind(TimePicker.TimeProperty, nameof(vm.AnchorTime))
                                // optional: clamp on ValueChanged to enforce a window
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
                    },
                    new Label().Text("Frequency").Font(size: 14, bold: true),
                    new FlexLayout
                    {
                        JustifyContent = FlexJustify.SpaceBetween,
                        Children =
                        {
                            CreateChip(Frequency.Daily),
                            CreateChip(Frequency.Weekly),
                            CreateChip(Frequency.Monthly),
                        }
                    },
                    new Label().Bind(Label.TextProperty, nameof(vm.IntervalDisplay)).Font(size: 18),
                    new Slider(1, 52, 1).Bind(Slider.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay),
                    new Stepper(1, 100, 1, 1).Bind(Stepper.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay),
                    Section("Day of Month",
                        new Entry { Keyboard = Keyboard.Numeric }
                            .Bind(Entry.TextProperty, nameof(vm.DayOfMonth))
                    ).Bind(IsVisibleProperty, nameof(vm.ShowDayOfMonth)),
                    Section("Cron Preview",
                        new Label { FontSize = 14, TextColor = Colors.Gray }
                            .Bind(Label.TextProperty, nameof(vm.CronPreview))
                    ),
                    new Label { TextColor = Colors.Red }
                        .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                        .Bind(IsVisibleProperty, nameof(vm.HasError)),
                    new Button { Text = "Save Job", CornerRadius = 8 }
                        .Bind()
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
        base.OnAppearing();
        await ViewModel.LoadCommand.ExecuteAsync(CustomerId);
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