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
        Title = "Schedule Job";

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

                    Section("Day of Month",
                        new Entry { Keyboard = Keyboard.Numeric }
                            .Bind(Entry.TextProperty, nameof(vm.DayOfMonth))
                    ),

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
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                }
            }
        };

        // Use LoadCommand not LoadAsync
        Loaded += async (_, _) => await ViewModel.LoadCommand.ExecuteAsync(CustomerId);
    }

    Button CreateChip(Frequency freq) =>
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

    static View Section(string label, View control) =>
        new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label().Text(label).Font(size: 14, bold: true),
                control
            }
        };
}