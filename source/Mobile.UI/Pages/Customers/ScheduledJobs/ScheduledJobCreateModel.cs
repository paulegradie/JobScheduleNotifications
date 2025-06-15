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
    [ObservableProperty] private CustomerDto? _selectedCustomer;
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


    public new bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Validation properties for individual fields
    public bool IsCustomerValid => SelectedCustomer != null;
    public bool IsTitleValid => !string.IsNullOrWhiteSpace(Title);
    public bool IsDescriptionValid => !string.IsNullOrWhiteSpace(Description);
    public bool IsCronValid => !string.IsNullOrWhiteSpace(CronPreview) && CronPreview != "Invalid parameters";

    // Validation error messages
    public string CustomerError => IsCustomerValid ? string.Empty : "Please select a customer";
    public string TitleError => IsTitleValid ? string.Empty : "Job title is required";
    public string DescriptionError => IsDescriptionValid ? string.Empty : "Job description is required";
    public string CronError => IsCronValid ? string.Empty : "Schedule configuration is invalid";

    // Overall validation message
    public string ValidationMessage
    {
        get
        {
            if (CanSave) return string.Empty;
            if (IsBusy) return "Please wait...";

            var missingFields = new List<string>();
            if (!IsCustomerValid) missingFields.Add("Customer");
            if (!IsTitleValid) missingFields.Add("Title");
            if (!IsDescriptionValid) missingFields.Add("Description");
            if (!IsCronValid) missingFields.Add("Schedule");

            return missingFields.Count > 0
                ? $"Please fill in: {string.Join(", ", missingFields)}"
                : string.Empty;
        }
    }

    public bool CanSave =>
        !IsBusy &&
        IsCustomerValid &&
        IsTitleValid &&
        IsDescriptionValid &&
        IsCronValid;

    public ScheduledJobCreateModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        UpdateCronPreview();
    }

    /// <summary>
    /// Clears all form fields to prepare for new job creation
    /// </summary>
    [RelayCommand]
    public void ClearFields()
    {
        Title = string.Empty;
        Description = string.Empty;
        AnchorDate = DateTime.Now;
        AnchorTime = TimeSpan.Zero;
        Frequency = Frequency.Daily;
        Interval = 1;
        DayOfMonth = null;
        ErrorMessage = string.Empty;

        // Update cron preview after clearing fields
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
                    var dayOfMonth = DayOfMonth ?? 1; // Default to 1st of month if not specified
                    builder.OnDayOfMonth(dayOfMonth).EveryMonths(Interval);
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

        // Notify that CanSave might have changed since it depends on CronPreview
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await RunWithSpinner(async () =>
        {
            var dto = new CreateScheduledJobDefinitionDto
            {
                CustomerId = SelectedCustomer!.Id,
                Title = Title,
                Description = Description,
                AnchorDate = AnchorDateTime,
                CronExpression = CronPreview
            };

            var result = await _jobRepository.CreateJobAsync(dto);

            if (result.IsSuccess)
            {
                await ShowSuccessAsync("Job Created", "Job has been scheduled successfully!");
                // Clear fields after successful creation
                ClearFields();
                // Navigate back to customer list
                await Navigation.NavigateToCustomerListAsync();
            }
            else
            {
                await ShowErrorAsync($"Failed to create job: {result.ErrorMessage ?? "Unknown error"}");
            }
        }, "Unable to create scheduled job");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        try
        {
            // Clear fields when cancelling
            ClearFields();
            await Navigation.NavigateToCustomerListAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Navigation error: {ex.Message}");
        }
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

    partial void OnCronPreviewChanged(string oldValue, string newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    partial void OnSelectedCustomerChanged(CustomerDto? oldValue, CustomerDto? newValue)
        => SaveCommand.NotifyCanExecuteChanged();

    public Style ChipStyle(object value, object parameter)
    {
        var freq = (Frequency)parameter;
        return freq == Frequency ? Device.Styles.BodyStyle : Device.Styles.CaptionStyle;
    }
}