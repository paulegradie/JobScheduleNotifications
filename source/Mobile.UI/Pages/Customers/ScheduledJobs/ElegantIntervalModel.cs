using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;


public partial class ElegantScheduleViewModel : ObservableObject
{
    [ObservableProperty] private Frequency _selectedFrequency = Frequency.Daily;
    [ObservableProperty] private int _interval = 1;
    [ObservableProperty] private DateTime _startDate = DateTime.Today;
    [ObservableProperty] private TimeSpan _startTime = TimeSpan.FromHours(9);
    [ObservableProperty] private string _cronExpression = string.Empty;

    public string IntervalDisplay =>
        $"Every {Interval} {SelectedFrequency.ToString().ToLower()}{(Interval>1?"s":"")}";

    public bool IsCustom => SelectedFrequency == Frequency.Custom;

    public bool CanSave => Interval >= 1 &&
        (!IsCustom || !string.IsNullOrWhiteSpace(CronExpression));

    public Style ChipStyle(object value, object parameter)
    {
        // parameter is the enum value for the chip
        return (Frequency)parameter == SelectedFrequency
            ? Device.Styles.BodyStyle
            : Device.Styles.CaptionStyle;
    }

    [RelayCommand]
    private void SelectFrequency(Frequency freq)
    {
        SelectedFrequency = freq;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        // Persist and close
    }
}