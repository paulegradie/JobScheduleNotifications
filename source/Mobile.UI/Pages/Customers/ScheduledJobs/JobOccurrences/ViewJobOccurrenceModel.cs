using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public record JobOccurrenceDetails(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId, JobOccurrenceId JobOccurrenceId);

public partial class ViewJobOccurrenceModel : BaseViewModel
{
    private readonly IJobOccurrenceRepository _repo;
    private readonly INavigationRepository _navigationRepository;

    [ObservableProperty] private string _jobTitle;
    [ObservableProperty] private DateTime _occurrenceDate;
    [ObservableProperty] private DateTime? _completedDate;
    [ObservableProperty] private bool _canMarkComplete;

    public ViewJobOccurrenceModel(IJobOccurrenceRepository repo, INavigationRepository navigationRepository)
    {
        _repo = repo;
        _navigationRepository = navigationRepository;
    }

    [RelayCommand]
    private async Task MarkCompletedAsync(JobOccurrenceDetails details)
    {
        await RunWithSpinner(async () =>
        {
            var success = await _repo
                .MarkOccurrenceAsCompletedAsync(details.CustomerId, details.ScheduledJobDefinitionId, details.JobOccurrenceId, CancellationToken.None);

            if (success.IsSuccess) return;
            await _navigationRepository.ShowAlertAsync("Job Done!", "The job has been marked as complete.");
        });
    }

    [RelayCommand]
    private async Task LoadAsync(JobOccurrenceDetails details)
    {
        await RunWithSpinner(async () =>
        {
            var resp = await _repo
                .GetOccurrenceByIdAsync(details.CustomerId, details.ScheduledJobDefinitionId, details.JobOccurrenceId, CancellationToken.None);
            if (!resp.IsSuccess) return;

            var dto = resp.Value.JobOccurrence;
            JobTitle = dto.JobTitle;
            OccurrenceDate = dto.OccurrenceDate;
            CompletedDate = dto.CompletedDate;
            CanMarkComplete = dto.CompletedDate == null;
        });
    }
}