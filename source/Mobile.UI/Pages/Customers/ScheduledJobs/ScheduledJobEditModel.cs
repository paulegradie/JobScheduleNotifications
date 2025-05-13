using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobEditModel : BaseViewModel
{
    private readonly IJobService _jobService;
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private ScheduledJobDefinitionDto _scheduledJobDefinitionDtoItem;
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _description;
    [ObservableProperty] private DateTime _anchorDate;
    [ObservableProperty] private Frequency _frequency;
    [ObservableProperty] private int _interval;
    [ObservableProperty] private int? _dayOfMonth;
    [ObservableProperty] private string _cronExpression;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private ObservableCollection<WeekDay> _selectedWeekDays = [];

    [ObservableProperty] private CustomerId _customerId;
    [ObservableProperty] private ScheduledJobDefinitionId _scheduledJobDefinitionId;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool CanSave =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(Title)
        && !string.IsNullOrWhiteSpace(Description)
        && DateTime.TryParse(AnchorDate.ToString(), out _);

    public ScheduledJobEditModel(
        IJobService jobService,
        ICustomerRepository customerRepository,
        INavigationRepository navigation)
    {
        _jobService = jobService;
        _customerRepository = customerRepository;
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task LoadForEditAsync(Details details)
    {
        await RunWithSpinner(async () =>
        {
            var dto = await _jobService.GetJobAsync(
                details.CustomerId, details.ScheduledJobDefinitionId);

            ScheduledJobDefinitionDtoItem = dto;
            CustomerId = dto.CustomerId;
            ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
            Title = dto.Title;
            Description = dto.Description;
            AnchorDate = dto.AnchorDate;
            CronExpression = dto.CronExpression;
        });
    }

    partial void OnTitleChanged(string oldVal, string newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnDescriptionChanged(string oldVal, string newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnAnchorDateChanged(DateTime oldVal, DateTime newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnFrequencyChanged(Frequency oldVal, Frequency newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnIntervalChanged(int oldVal, int newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnDayOfMonthChanged(int? oldVal, int? newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnCronExpressionChanged(string oldVal, string newVal)
        => SaveCommand.NotifyCanExecuteChanged();

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (ScheduledJobDefinitionDtoItem is null) return;

        await RunWithSpinner(async () =>
        {
            var updatedDto = new ScheduledJobDefinitionDto(
                CustomerId: CustomerId,
                ScheduledJobDefinitionId: ScheduledJobDefinitionId,
                AnchorDate: AnchorDate,
                CronExpression: CronExpression,
                JobOccurrences: [],
                Title: Title,
                Description: Description
            );

            await _jobService.UpdateJobAsync(updatedDto);
            await _navigation.GoBackAsync();
        });
    }
}