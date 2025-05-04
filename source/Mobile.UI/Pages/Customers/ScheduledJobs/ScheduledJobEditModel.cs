using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(CustomerId), "customerId")]
[QueryProperty(nameof(ScheduledJobDefinitionId), "scheduledJobDefinitionId")]
public partial class ScheduledJobEditModel : BaseViewModel
{
    private readonly IJobService _jobService;
    private readonly ICustomerService _customerService;
    private readonly INavigationRepository _navigation;

    public ScheduledJobEditModel(IJobService jobService, ICustomerService customerService, INavigationRepository navigation)
    {
        _jobService = jobService;
        _customerService = customerService;
        _navigation = navigation;
    }

    [ObservableProperty] private ScheduledJobDefinitionDto _scheduledJobDefinitionDtoItem;
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _description;
    [ObservableProperty] private string _anchorDate;
    [ObservableProperty] private string _frequency;
    [ObservableProperty] private string _interval;
    [ObservableProperty] private string _dayOfMonth;
    [ObservableProperty] private string _cronExpression;
    [ObservableProperty] private ObservableCollection<WeekDay> _selectedWeekDays;

    [ObservableProperty] private CustomerId _customerId;
    [ObservableProperty] private ScheduledJobDefinitionId _scheduledJobDefinitionId;


    [RelayCommand]
    private async Task LoadForEditAsync()
    {
        if (IsBusy) return;

        await RunSafeAsync(async () =>
        {
            // fetch the existing record
            var dto = await _jobService.GetJobAsync(CustomerId, ScheduledJobDefinitionId);
            ScheduledJobDefinitionDtoItem = dto;

            // push all the fields into your bindable props:
            Title = dto.Title;
            Description = dto.Description;
            AnchorDate = dto.AnchorDate.ToString("yyyy-MM-dd");
            Frequency = dto.Pattern.Frequency.ToString();
            Interval = dto.Pattern.Interval.ToString();
            DayOfMonth = dto.Pattern.DayOfMonth?.ToString() ?? string.Empty;
            CronExpression = dto.Pattern.CronExpression ?? string.Empty;

            SelectedWeekDays.Clear();
            foreach (var wd in dto.Pattern.WeekDays)
                SelectedWeekDays.Add(wd);
        });
    }


    [RelayCommand]
    private async Task SaveAsync()
    {
        // guard
        if (ScheduledJobDefinitionDtoItem is null) return;

        await RunSafeAsync(async () =>
        {
            // 1) parse your string fields
            if (!DateTime.TryParse(AnchorDate, out var anchor))
                throw new InvalidOperationException("Invalid anchor date");

            if (!Enum.TryParse<Frequency>(Frequency, true, out var freq))
                throw new InvalidOperationException("Invalid frequency");

            if (!int.TryParse(Interval, out var interval))
                throw new InvalidOperationException("Invalid interval");

            int? dayOfMonthParsed = null;
            if (!string.IsNullOrWhiteSpace(DayOfMonth))
            {
                if (int.TryParse(DayOfMonth, out var dom))
                    dayOfMonthParsed = dom;
                else
                    throw new InvalidOperationException("Invalid day of month");
            }

            // 2) build a new RecurrencePatternDto
            var patternDto = new RecurrencePatternDto
            {
                Frequency = freq,
                Interval = interval,
                WeekDays = SelectedWeekDays.ToArray(),
                DayOfMonth = dayOfMonthParsed,
                CronExpression = string.IsNullOrWhiteSpace(CronExpression)
                    ? null
                    : CronExpression
            };

            var updated = new ScheduledJobDefinitionDto
            (
                CustomerId: ScheduledJobDefinitionDtoItem.CustomerId,
                ScheduledJobDefinitionId: ScheduledJobDefinitionDtoItem.ScheduledJobDefinitionId,
                AnchorDate: anchor,
                Pattern: patternDto,
                [],
                Title: Title,
                Description: Description
            );

            // 4) call your service
            await _jobService.UpdateJobAsync(updated);

            // 5) navigate back (or to wherever you need)
            await _navigation.GoBackAsync();
        });
    }
}