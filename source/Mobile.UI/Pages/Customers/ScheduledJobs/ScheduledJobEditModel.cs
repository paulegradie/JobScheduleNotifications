using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Cron;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobEditModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();
    [ObservableProperty] private CustomerDto _selectedCustomer;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private DateTime _anchorDate = DateTime.Now;
    [ObservableProperty] private Frequency _frequency = Frequency.Daily;
    [ObservableProperty] private int _interval = 1;
    [ObservableProperty] private int? _dayOfMonth;
    [ObservableProperty] private string _cronExpression = string.Empty;
    [ObservableProperty] private string _cronPreview;
    [ObservableProperty] private string _errorMessage;

    private ScheduledJobDefinitionDto ScheduledJobDefinitionDtoItem { get; set; }
    private CustomerId CustomerId { get; set; }
    private ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);


    public bool IsCustom => Frequency == Frequency.Custom;

    public string IntervalDisplay =>
        $"Every {Interval} {Frequency.ToString().ToLower()}{(Interval > 1 ? "s" : "")}";

    public bool CanSave =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Title) &&
        !string.IsNullOrWhiteSpace(Description) &&
        (Frequency != Frequency.Custom || !string.IsNullOrWhiteSpace(CronExpression));

    public ScheduledJobEditModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository,
        INavigationRepository navigation)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task LoadForEditAsync(Details details)
    {
        await RunWithSpinner(async () =>
        {
            var result = await _jobRepository.GetJobByIdAsync(details.CustomerId, details.ScheduledJobDefinitionId);
            if (result.IsSuccess)
            {
                var dto = result.Value;
                ScheduledJobDefinitionDtoItem = dto;
                CustomerId = dto.CustomerId;

                ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
                Title = dto.Title;
                Description = dto.Description;
                AnchorDate = dto.AnchorDate;
                CronExpression = dto.CronExpression;
                DayOfMonth = dto.DayOfMonth;
            }
        });
    }


    [RelayCommand]
    private void SelectFrequency(Frequency freq)
    {
        Frequency = freq;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (ScheduledJobDefinitionDtoItem is null) return;

        await RunWithSpinner(async () =>
        {
            var updatedDto = new UpdateJobDto(
                CustomerId: CustomerId,
                ScheduledJobDefinitionId: ScheduledJobDefinitionId,
                AnchorDate: AnchorDate,
                CronExpression: CronExpression,
                Title: Title,
                Description: Description,
                DayOfMonth: DayOfMonth
            );

            await _jobRepository.UpdateJobAsync(CustomerId, ScheduledJobDefinitionId, updatedDto, CancellationToken.None);
            await _navigation.ShowAlertAsync("Success", "Job Updated!");
            await _navigation.GoBackAsync();
        });
    }

    partial void OnTitleChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnDescriptionChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnIntervalChanged(int oldValue, int newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        UpdateCronPreview();
    }

    partial void OnCronExpressionChanged(string oldValue, string newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        UpdateCronPreview();
    }

    partial void OnDayOfMonthChanged(int oldValue, int newvValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        UpdateCronPreview();
    }


    partial void OnFrequencyChanged(Frequency value)
    {
        SaveCommand.NotifyCanExecuteChanged();
        UpdateCronPreview();
    }

    public Style ChipStyle(object value, object parameter)
    {
        var freq = (Frequency)parameter;
        return freq == Frequency ? Device.Styles.BodyStyle : Device.Styles.CaptionStyle;
    }

    private void UpdateCronPreview()
    {
        try
        {
            if (IsCustom)
            {
                CronPreview = CronExpression;
            }
            else
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
                    case Frequency.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                CronPreview = builder.Build().ToString();
            }
        }
        catch
        {
            CronPreview = "Invalid parameters";
        }

        // Notify changes
        OnPropertyChanged(nameof(CronPreview));
        OnPropertyChanged(nameof(IntervalDisplay));
        OnPropertyChanged(nameof(IsCustom));
    }
}