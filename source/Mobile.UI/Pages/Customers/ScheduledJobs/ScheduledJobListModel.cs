using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;
using Guid = System.Guid;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobListModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly INavigationRepository _navigation;

    public ScheduledJobListModel(INavigationRepository navigation, IJobRepository jobRepository)
    {
        _navigation = navigation;
        _jobRepository = jobRepository;
    }

    [ObservableProperty] private ObservableCollection<ScheduledJobDefinitionDto> _scheduledJobs = new();
    public CustomerId CustomerId { get; set; }
    private string? _currentCustomerIdString;

    [RelayCommand]
    private async Task LoadForCustomerAsync(string customerIdString)
    {
        _currentCustomerIdString = customerIdString;

        var guid = await RunWithSpinner(async () =>
        {
            if (!Guid.TryParse(customerIdString, out var guid))
            {
                ErrorMessage = "Invalid customer ID.";
            }

            return guid;
        });
        var customerId = new CustomerId(guid);
        CustomerId = customerId; // check this isn't null?

        await RunWithSpinner(async () =>
        {
            var result = await _jobRepository.GetJobsByCustomerAsync(customerId);
            if (result.IsSuccess)
            {
                var jobs = result.Value;
                ScheduledJobs.Clear();
                foreach (var scheduledJobDefinitionDto in jobs)
                {
                    ScheduledJobs.Add(scheduledJobDefinitionDto);
                }
            }
        }, "Failed to load scheduled jobs.");
        OnPropertyChanged(nameof(ScheduledJobs));
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

    [RelayCommand]
    private async Task NavigateToViewAsync(ScheduledJobDefinitionId scheduledJobDefinitionId)
    {
        await _navigation.GoToAsync(nameof(ScheduledJobViewPage), new Dictionary<string, object>
        {
            ["ScheduledJobDefinitionId"] = scheduledJobDefinitionId.Value.ToString(),
            ["CustomerId"] = CustomerId.Value.ToString()
        });
    }
}