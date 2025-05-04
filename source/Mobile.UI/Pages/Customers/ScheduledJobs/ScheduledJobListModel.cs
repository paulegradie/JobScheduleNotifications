using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobListModel : BaseViewModel
{
    private readonly IJobService _jobService;
    private readonly INavigationRepository _navigation;

    public ScheduledJobListModel(INavigationRepository navigation, IJobService jobService)
    {
        _navigation = navigation;
        _jobService = jobService;
    }

    [ObservableProperty] private ObservableCollection<ScheduledJobDefinitionDto> _scheduledJobs = new();

    [RelayCommand]
    private async Task LoadForCustomerAsync(string customerIdString)
    {
        if (IsBusy) return;

        if (!Guid.TryParse(customerIdString, out var guid))
        {
            ErrorMessage = "Invalid customer ID.";
            return;
        }

        var customerId = new CustomerId(guid);

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var jobs = await _jobService.GetJobsAsync(customerId);
            ScheduledJobs.Clear();
            foreach (var j in jobs)
                ScheduledJobs.Add(j);
        }
        catch
        {
            ErrorMessage = "Failed to load scheduled jobs.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToEditAsync(ScheduledJobDefinitionDto dto)
    {
        if (dto == null) return;
        await _navigation.GoToAsync(nameof(ScheduledJobEditPage), new Dictionary<string, object>
        {
            ["scheduledJobDefinitionId"] = dto.ScheduledJobDefinitionId.Value.ToString()
        });
    }
}