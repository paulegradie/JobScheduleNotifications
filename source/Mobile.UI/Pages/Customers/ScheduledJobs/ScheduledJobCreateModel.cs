using System.Collections.ObjectModel;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Cron;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobCreateModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();
    [ObservableProperty] private CustomerDto _selectedCustomer;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private DateTime _anchorDate = DateTime.Now;
    [ObservableProperty] private TimeSpan _anchorTime; // bound by TimePicker

    // and when you read it out:
    public DateTime AnchorDateTime => AnchorDate.Date + AnchorTime;

    [ObservableProperty] private Frequency _frequency = Frequency.Daily;
    [ObservableProperty] private int _interval = 1;
    [ObservableProperty] private int? _dayOfMonth;
    [ObservableProperty] private string _cronExpression = string.Empty;
    [ObservableProperty] private string _cronPreview;
    [ObservableProperty] private string _errorMessage;

    public string IntervalDisplay => FormatIntervalDisplay();

    private string FormatIntervalDisplay()
    {
        var freqString = Frequency switch
        {
            Frequency.Daily => "day",
            Frequency.Weekly => "week",
            Frequency.Monthly => "month",
            _ => ""
        };

        return $"Every {Interval} {freqString}{(Interval > 1 ? "s" : "")}";
    }


    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool CanSave =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Title) &&
        !string.IsNullOrWhiteSpace(Description) &&
        !string.IsNullOrWhiteSpace(CronPreview);

    public ScheduledJobCreateModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        UpdateCronPreview();
    }

    partial void OnFrequencyChanged(Frequency value) => UpdateCronPreview();

    partial void OnIntervalChanged(int value) => UpdateCronPreview();

    partial void OnDayOfMonthChanged(int? value) => UpdateCronPreview();

    partial void OnAnchorDateChanged(DateTime value) => UpdateCronPreview();

    [RelayCommand]
    private async Task LoadAsync(string customerId)
    {
        await RunWithSpinner(async () =>
        {
            var result = await _customerRepository.GetCustomersAsync();
            if (result.IsSuccess)
            {
                Customers.Clear();
                foreach (var c in result.Value)
                    Customers.Add(c);
                SelectedCustomer = Customers.FirstOrDefault(c => c.Id.Value.ToString() == customerId)
                                   ?? Customers.FirstOrDefault();
            }
        });
    }

    public bool ShowDayOfMonth => Frequency == Frequency.Monthly;

    [RelayCommand]
    private void SelectFrequency(Frequency freq)
    {
        Frequency = freq;
        OnPropertyChanged(nameof(Frequency));
        OnPropertyChanged(nameof(ShowDayOfMonth));
    }

    private void UpdateCronPreview()
    {
        try
        {
            var builder = FluentCron.Create()
                .AtMinute(0)
                .AtHour(0);

            switch (Frequency)
            {
                case Frequency.Daily:
                    builder.EveryDays(Interval);
                    break;
                case Frequency.Weekly:
                    builder.EveryWeeks(Interval);
                    break;
                case Frequency.Monthly:
                    if (DayOfMonth.HasValue)
                        builder.OnDayOfMonth(DayOfMonth.Value).EveryMonths(Interval);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CronPreview = builder.Build().ToString();
        }

        catch
        {
            CronPreview = "Invalid parameters";
        }

        // Notify changes
        OnPropertyChanged(nameof(CronPreview));
        OnPropertyChanged(nameof(CronExpression));
        OnPropertyChanged(nameof(IntervalDisplay));
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await RunWithSpinner(async () =>
        {
            var dto = new CreateScheduledJobDefinitionDto
            {
                CustomerId = SelectedCustomer.Id,
                Title = Title,
                Description = Description,
                AnchorDate = AnchorDateTime,
                CronExpression = CronPreview
            };

            await _jobRepository.CreateJobAsync(dto);

            await ShowSuccessAsync("Job scheduled!");

            await Navigation.NavigateToCustomerListAsync();

        });
    }

    partial void OnTitleChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnDescriptionChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnFrequencyChanged(Frequency oldValue, Frequency newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnIntervalChanged(int oldValue, int newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnCronExpressionChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    public Style ChipStyle(object value, object parameter)
    {
        var freq = (Frequency)parameter;
        return freq == Frequency ? Device.Styles.BodyStyle : Device.Styles.CaptionStyle;
    }
}