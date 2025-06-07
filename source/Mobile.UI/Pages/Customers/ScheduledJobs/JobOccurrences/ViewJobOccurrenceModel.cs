using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public partial class ViewJobOccurrenceModel : BaseViewModel
{
    private readonly IJobOccurrenceRepository _repo;
    private readonly INavigationRepository _navigationRepository;
    private readonly IJobPhotoRepository _photoRepository;

    [ObservableProperty] private string _jobTitle;
    [ObservableProperty] private string _jobDescription;
    [ObservableProperty] private DateTime _occurrenceDate;
    [ObservableProperty] private DateTime? _completedDate;
    [ObservableProperty] private bool _canMarkComplete;
    [ObservableProperty] private bool _markedAsComplete;
    [ObservableProperty] private ICollection<JobReminderDto> _jobReminderDtos;

    public ViewJobOccurrenceModel(IJobOccurrenceRepository repo, INavigationRepository navigationRepository, IJobPhotoRepository photoRepository)
    {
        _repo = repo;
        _navigationRepository = navigationRepository;
        _photoRepository = photoRepository;
    }

    private CustomerJobAndOccurrenceIds? CustomerJobAndOccurrenceIds { get; set; }

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
            MarkedAsComplete = dto.MarkedAsCompleted;
        });
    }

    [RelayCommand]
    private async Task MarkCompletedAsync()
    {
        var ids = CustomerJobAndOccurrenceIds;

        if (ids == null) return;
        await RunWithSpinner(async () =>
        {
            var success = await _repo
                .MarkOccurrenceAsCompletedAsync(ids.CustomerId, ids.ScheduledJobDefinitionId, ids.JobOccurrenceId, CancellationToken.None);

            if (!success.IsSuccess) return;

            MarkedAsComplete = true;
            CompletedDate = DateTime.Now;
            CanMarkComplete = false;

            // await LoadAsync(ids);
            OnPropertyChanged(nameof(CompletedDate));
            OnPropertyChanged(nameof(CanMarkComplete));
            OnPropertyChanged(nameof(MarkedAsComplete));

            await _navigationRepository.ShowAlertAsync("Job Done!", "The job has been marked as complete.");
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationRepository.GoBackAsync();
    }

    [RelayCommand]
    private async Task CreateInvoiceAsync()
    {
        if (CustomerJobAndOccurrenceIds == null) return;

        await _navigationRepository.GoToAsync(
            "InvoiceCreatePage",
            new Dictionary<string, object?>
            {
                { "CustomerId", CustomerJobAndOccurrenceIds.CustomerId.ToString() },
                { "ScheduledJobDefinitionId", CustomerJobAndOccurrenceIds.ScheduledJobDefinitionId.ToString() },
                { "JobOccurrenceId", CustomerJobAndOccurrenceIds.JobOccurrenceId.ToString() },
                { "JobDescription", JobDescription }
            });
    }

    [RelayCommand]
    private async Task UploadPhotoAsync()
    {
        if (CustomerJobAndOccurrenceIds is null)
            return;

        string action = await Application.Current.MainPage.DisplayActionSheet(
            "Upload Photo",
            "Cancel",
            null,
            "Take Photo",
            "Pick from Gallery");

        FileResult? photo = null;

        try
        {
            if (action == "Take Photo")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                }
                else
                {
                    await _navigationRepository.ShowAlertAsync("Camera Unavailable", "Camera is not supported on this device.");
                    return;
                }
            }
            else if (action == "Pick from Gallery")
            {
// #if WINDOWS
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a photo",
                    FileTypes = FilePickerFileType.Images
                });
                photo = result;
// #else
//             var options = new MediaPickerOptions { Title = "Pick a photo that you've previously taken" };
//             photo = await MediaPicker.Default.PickPhotoAsync(options);
// #endif
            }
        }
        catch (Exception ex)
        {
            await _navigationRepository.ShowAlertAsync("Error", $"An error occurred: {ex.Message}");
            return;
        }

        if (photo == null)
            return;

        var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

        await using (var sourceStream = await photo.OpenReadAsync())
        await using (var localFileStream = File.OpenWrite(localFilePath))
        {
            await sourceStream.CopyToAsync(localFileStream);
        }

        await RunWithSpinner(async () =>
        {
            var success = await _photoRepository.UploadPhotoAsync(
                localFilePath,
                CustomerJobAndOccurrenceIds.CustomerId,
                CustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
                CustomerJobAndOccurrenceIds.JobOccurrenceId);

            if (success)
                await _navigationRepository.ShowAlertAsync("Photo Uploaded", "The photo has been successfully uploaded.");
            else
                await _navigationRepository.ShowAlertAsync("Upload Failed", "Something went wrong uploading the photo.");
        });
    }
}