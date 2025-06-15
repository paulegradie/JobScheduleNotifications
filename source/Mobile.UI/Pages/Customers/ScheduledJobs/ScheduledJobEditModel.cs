﻿﻿﻿using System.Collections.ObjectModel;
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
    [ObservableProperty] private TimeSpan _anchorTime; // bound by TimePicker
    public DateTime AnchorDateTime => AnchorDate.Date + AnchorTime;


    [ObservableProperty] private Frequency _frequency = Frequency.Daily;
    [ObservableProperty] private int _interval = 1;
    [ObservableProperty] private int? _dayOfMonth;
    [ObservableProperty] private string _cronExpression = string.Empty;
    [ObservableProperty] private string _cronPreview;
    [ObservableProperty] private string _errorMessage;

    private ScheduledJobDefinitionDto ScheduledJobDefinitionDtoItem { get; set; }
    private CustomerId CustomerId { get; set; }
    private ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }


    public string IntervalDisplay => FormatIntervalDisplay();

    private string FormatIntervalDisplay()
    {
        var freqString = Frequency switch
        {
            Frequency.Daily => "daily",
            Frequency.Weekly => "weekly",
            Frequency.Monthly => "monthly",
            _ => ""
        };

        return $"Every {Interval} {freqString}{(Interval > 1 ? "s" : "")}";
    }

    // Validation properties for individual fields
    public bool IsTitleValid => !string.IsNullOrWhiteSpace(Title);
    public bool IsDescriptionValid => !string.IsNullOrWhiteSpace(Description);
    public bool IsCronExpressionValid => !string.IsNullOrWhiteSpace(CronExpression);

    // Validation error messages
    public string TitleError => IsTitleValid ? string.Empty : "Job title is required";
    public string DescriptionError => IsDescriptionValid ? string.Empty : "Job description is required";
    public string CronExpressionError => IsCronExpressionValid ? string.Empty : "Schedule configuration is invalid";

    // Overall validation message
    public string ValidationMessage
    {
        get
        {
            if (CanSave) return string.Empty;
            if (IsBusy) return "Please wait...";

            var missingFields = new List<string>();
            if (!IsTitleValid) missingFields.Add("Title");
            if (!IsDescriptionValid) missingFields.Add("Description");
            if (!IsCronExpressionValid) missingFields.Add("Schedule");

            return missingFields.Count > 0
                ? $"Please fill in: {string.Join(", ", missingFields)}"
                : string.Empty;
        }
    }

    public bool CanSave =>
        !IsBusy &&
        IsTitleValid &&
        IsDescriptionValid &&
        IsCronExpressionValid;

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
    private async Task LoadForEditAsync(LoadParametersCustomerIdAndScheduleJobDefId loadParametersCustomerIdAndScheduleJobDefId)
    {
        await RunWithSpinner(async () =>
        {
            var result = await _jobRepository.GetJobByIdAsync(loadParametersCustomerIdAndScheduleJobDefId.CustomerId, loadParametersCustomerIdAndScheduleJobDefId.ScheduledJobDefinitionId);
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
            await ShowSuccessAsync("Job Updated!");
            await GoBackAsync();
        });
    }

    partial void OnTitleChanged(string oldValue, string newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsTitleValid));
        OnPropertyChanged(nameof(TitleError));
        OnPropertyChanged(nameof(ValidationMessage));
    }

    partial void OnDescriptionChanged(string oldValue, string newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsDescriptionValid));
        OnPropertyChanged(nameof(DescriptionError));
        OnPropertyChanged(nameof(ValidationMessage));
    }

    partial void OnIntervalChanged(int oldValue, int newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        UpdateCronPreview();
    }

    partial void OnCronExpressionChanged(string oldValue, string newValue)
    {
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsCronExpressionValid));
        OnPropertyChanged(nameof(CronExpressionError));
        OnPropertyChanged(nameof(ValidationMessage));
        UpdateCronPreview();
    }

    partial void OnDayOfMonthChanged(int? oldValue, int? newValue)
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
        OnPropertyChanged(nameof(IntervalDisplay));
    }
}