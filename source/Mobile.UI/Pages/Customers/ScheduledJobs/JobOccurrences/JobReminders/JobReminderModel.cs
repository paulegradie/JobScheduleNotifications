using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences.JobReminders;

/// <summary>
/// ViewModel for a single Job Reminder.
/// </summary>
public partial class JobReminderModel : BaseViewModel
{
    private readonly IJobReminderRepository _repo;
    private readonly INavigationRepository _navigationRepository;

    [ObservableProperty] private DateTime _reminderDate;
    [ObservableProperty] private string _snapshottedDescription = string.Empty;

    private CustomerJobAndOccurrenceReminderIds? _ids;

    public JobReminderModel(IJobReminderRepository repo, INavigationRepository navigationRepository)
    {
        _repo = repo;
        _navigationRepository = navigationRepository;
    }

    [RelayCommand]
    private async Task LoadAsync(CustomerJobAndOccurrenceReminderIds ids)
    {
        _ids = ids;
        await RunWithSpinner(async () =>
        {
            var resp = await _repo
                .GetReminderAsync(ids.CustomerId, ids.ScheduledJobDefinitionId, ids.JobOccurrenceId, ids.JobReminderId, CancellationToken.None);

            if (!resp.IsSuccess) return;
            var dto = resp.Value.JobReminder;

            ReminderDate = dto.ReminderDate;
            SnapshottedDescription = dto.SnapshottedDescription;
        });
    }
}