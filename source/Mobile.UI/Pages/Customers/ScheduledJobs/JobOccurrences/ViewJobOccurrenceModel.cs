using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public partial class ViewJobOccurrenceModel : BaseViewModel
{
    private readonly IJobOccurrenceRepository _repo;
    private readonly INavigationRepository _navigationRepository;

    [ObservableProperty] private string _jobTitle;
    [ObservableProperty] private string _jobDescription;
    [ObservableProperty] private DateTime _occurrenceDate;
    [ObservableProperty] private DateTime? _completedDate;
    [ObservableProperty] private bool _canMarkComplete;
    [ObservableProperty] private bool _markedAsComplete;
    [ObservableProperty] private ICollection<JobReminderDto> _jobReminderDtos;


    public ViewJobOccurrenceModel(IJobOccurrenceRepository repo, INavigationRepository navigationRepository)
    {
        _repo = repo;
        _navigationRepository = navigationRepository;
    }

    [RelayCommand]
    private async Task LoadAsync(CustomerJobAndOccurrenceIds ids)
    {
        CustomerJobAndOccurrenceIds = ids;
        await RunWithSpinner(async () =>
        {
            var resp = await _repo
                .GetOccurrenceByIdAsync(ids.CustomerId, ids.ScheduledJobDefinitionId, ids.JobOccurrenceId, CancellationToken.None);
            if (!resp.IsSuccess) return;

            var dto = resp.Value.JobOccurrence;
            OccurrenceDate = dto.OccurrenceDate;
            CompletedDate = dto.CompletedDate;
            CanMarkComplete = dto.CompletedDate == null;
            JobTitle = dto.JobTitle;
            JobDescription = dto.JobDescription;
            JobReminderDtos = dto.JobReminders;
            MarkedAsComplete = dto.MarkedAsCompleted;
        });
    }

    [RelayCommand]
    private async Task MarkCompletedAsync(CustomerJobAndOccurrenceIds id)
    {
        var ids = CustomerJobAndOccurrenceIds;

        if (ids == null) return;
        await RunWithSpinner(async () =>
        {
            var success = await _repo
                .MarkOccurrenceAsCompletedAsync(id.CustomerId, id.ScheduledJobDefinitionId, id.JobOccurrenceId, CancellationToken.None);

            if (success.IsSuccess) return;
            await _navigationRepository.ShowAlertAsync("Job Done!", "The job has been marked as complete.");
        });
    }

    private CustomerJobAndOccurrenceIds? CustomerJobAndOccurrenceIds { get; set; }
}