using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobListModel : Base.BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly INavigationRepository _navigation;

    public ScheduledJobListModel(INavigationRepository navigation, IJobRepository jobRepository)
    {
        _navigation = navigation;
        _jobRepository = jobRepository;
    }

    [ObservableProperty] private ObservableCollection<ScheduledJobDefinitionDto> _scheduledJobs = [];

    [RelayCommand]
    private async Task LoadForCustomerAsync(string customerIdString)
    {
        if (IsBusy) return;
        var guid = await RunWithSpinner(async () =>
        {
            if (!Guid.TryParse(customerIdString, out var guid))
            {
                ErrorMessage = "Invalid customer ID.";
            }

            return guid;
        });


        await RunWithSpinner(async () =>
        {
            var customerId = new CustomerId(guid);
            var result = await _jobRepository.GetJobsByCustomerAsync(customerId);
            if (result.IsSuccess)
            {
                var jobs = result.Value;
                ScheduledJobs.Clear();
                foreach (var j in jobs)
                    ScheduledJobs.Add(j);
            }
        }, "Failed to load scheduled jobs.");
    }

    [RelayCommand]
    private async Task NavigateToEditAsync(ScheduledJobDefinitionDto? dto)
    {
        if (dto == null) return;
        await _navigation.GoToAsync(nameof(ScheduledJobEditPage), new Dictionary<string, object>
        {
            ["ScheduledJobDefinitionId"] = dto.ScheduledJobDefinitionId.Value.ToString(),
            ["CustomerId"] = dto.CustomerId.Value.ToString()
        });
    }
}