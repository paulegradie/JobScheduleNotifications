using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public class ElegantSchedulePage : BasePage<ElegantScheduleViewModel>
{
    public ElegantSchedulePage(ElegantScheduleViewModel vm) : base(vm)
    {
        BindingContext = vm;
        Title = "Elegant Schedule Event";

        // Preset frequency chips
        var chips = new FlexLayout { JustifyContent = FlexJustify.SpaceBetween, Margin = 20 };
        foreach (var freq in Enum.GetValues<Frequency>())
        {
            var chip = new Button
                {
                    Text = freq.ToString(),
                    CornerRadius = 20,
                    Padding = new Thickness(16, 8),
                    Style = (
                        freq == ViewModel.SelectedFrequency
                            ? Device.Styles.BodyStyle
                            : Device.Styles.CaptionStyle
                    ),
                    Command = vm.SelectFrequencyCommand,
                }
                .Bind(Button.CommandParameterProperty, ".")
                .Bind(Button.CommandParameterProperty, ".", source: freq)
                .Bind(Button.StyleProperty, nameof(vm.ChipStyle), converterParameter: freq);
            chips.Children.Add(chip);
        }

        // Slider + interval label
        var slider = new Slider(1, 52, vm.Interval)
            .Bind(Slider.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay);

        var intervalLabel = new Label()
            .Bind(Label.TextProperty, nameof(vm.IntervalDisplay))
            .FontSize(18)
            .Margin(0, 8);

        // Fallback stepper
        var stepper = new Stepper(1, 100, 1, 1)
            .Bind(Stepper.ValueProperty, nameof(vm.Interval), BindingMode.TwoWay);

        // Date & time pickers
        var datePicker = new DatePicker()
            .Bind(DatePicker.DateProperty, nameof(vm.StartDate), BindingMode.TwoWay);

        var timePicker = new TimePicker()
            .Bind(TimePicker.TimeProperty, nameof(vm.StartTime), BindingMode.TwoWay);

        // Custom Cron field
        var cronEntry = new Entry { Placeholder = "Cron expression" }
            .Bind(Entry.TextProperty, nameof(vm.CronExpression));

        var cronSection = new StackLayout
        {
            Children = { new Label().Text("Advanced (Cron)"), cronEntry },
            IsVisible = false
        }.Bind(IsVisibleProperty, nameof(vm.IsCustom));

        // Save button
        var save = new Button { Text = "Save", CornerRadius = 8, HeightRequest = 50 }
            .Bind(Button.CommandProperty, nameof(vm.SaveCommand))
            .Bind(IsEnabledProperty, nameof(vm.CanSave));

        // Layout grid
        Content = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Chips, Auto),
                (Row.Slider, Auto),
                (Row.Label, Auto),
                (Row.Stepper, Auto),
                (Row.Date, Auto),
                (Row.Time, Auto),
                (Row.Cron, Auto),
                (Row.Save, Auto)
            ),
            Children =
            {
                chips.Row(Row.Chips),
                slider.Row(Row.Slider).Margin(20, 0),
                intervalLabel.Row(Row.Label).CenterHorizontal(),
                stepper.Row(Row.Stepper).CenterHorizontal(),
                datePicker.Row(Row.Date).Margin(20, 0),
                timePicker.Row(Row.Time).Margin(20, 0),
                cronSection.Row(Row.Cron).Margin(20, 0),
                save.Row(Row.Save).Margin(20, 20)
            }
        };
    }

    enum Row
    {
        Chips,
        Slider,
        Label,
        Stepper,
        Date,
        Time,
        Cron,
        Save
    }
}