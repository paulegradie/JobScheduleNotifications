using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Navigation;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;
using Type = Android.Renderscripts.Type;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public partial class ViewJobOccurrenceModel : BaseViewModel
{
    private readonly IJobOccurrenceRepository _jobOccurrenceRepository;
    private readonly IJobCompletedPhotoRepository _completedPhotoRepository;

    [ObservableProperty] private string _jobTitle;
    [ObservableProperty] private string _jobDescription;
    [ObservableProperty] private DateTime _occurrenceDate;
    [ObservableProperty] private DateTime? _completedDate;
    [ObservableProperty] private bool _canMarkComplete;
    [ObservableProperty] private bool _markedAsComplete;
    [ObservableProperty] private ICollection<JobReminderDto> _jobReminderDtos;


    [ObservableProperty] private ObservableCollection<PhotoDisplayItem> _photoPaths = new();

    public ViewJobOccurrenceModel(
        IJobOccurrenceRepository jobOccurrenceRepository,
        IJobCompletedPhotoRepository completedPhotoRepository)
    {
        _jobOccurrenceRepository = jobOccurrenceRepository;
        _completedPhotoRepository = completedPhotoRepository;
    }

    private CustomerJobAndOccurrenceIds? CustomerJobAndOccurrenceIds { get; set; }

    [RelayCommand]
    private async Task LoadAsync(CustomerJobAndOccurrenceIds ids)
    {
        CustomerJobAndOccurrenceIds = ids;
        await RunWithSpinner(async () =>
        {
            var resp = await _jobOccurrenceRepository
                .GetOccurrenceByIdAsync(ids.CustomerId, ids.ScheduledJobDefinitionId, ids.JobOccurrenceId, CancellationToken.None);
            if (!resp.IsSuccess) return;

            var dto = resp.Value.JobOccurrence;
            OccurrenceDate = dto.OccurrenceDate;
            CompletedDate = dto.CompletedDate;
            CanMarkComplete = dto.CompletedDate == null;
            JobTitle = dto.JobTitle;
            JobDescription = dto.JobDescription;
            MarkedAsComplete = dto.MarkedAsCompleted;

            foreach (var photo in dto.JobCompletedPhotosDto)
            {
                PhotoPaths.Add(new PhotoDisplayItem(photo.JobCompletedPhotoId, photo.PhotoUri));
            }
        });
    }

    [RelayCommand]
    private async Task MarkCompletedAsync()
    {
        var ids = CustomerJobAndOccurrenceIds;

        if (ids == null) return;
        await RunWithSpinner(async () =>
        {
            var success = await _jobOccurrenceRepository
                .MarkOccurrenceAsCompletedAsync(ids.CustomerId, ids.ScheduledJobDefinitionId, ids.JobOccurrenceId, CancellationToken.None);

            if (!success.IsSuccess) return;

            MarkedAsComplete = true;
            CompletedDate = DateTime.Now;
            CanMarkComplete = false;

            OnPropertyChanged(nameof(CompletedDate));
            OnPropertyChanged(nameof(CanMarkComplete));
            OnPropertyChanged(nameof(MarkedAsComplete));

            await ShowSuccessAsync("The job has been marked as complete.");
        });
    }


    [RelayCommand]
    private async Task CreateInvoiceAsync()
    {
        if (CustomerJobAndOccurrenceIds == null) return;

        await Navigation.NavigateToInvoiceCreateAsync(new InvoiceCreateParameters(
            CustomerJobAndOccurrenceIds.CustomerId,
            CustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
            CustomerJobAndOccurrenceIds.JobOccurrenceId));
    }

    [RelayCommand]
    private async Task UploadPhotoAsync()
    {
        if (CustomerJobAndOccurrenceIds is null)
            return;

        var action = await Application.Current.MainPage.DisplayActionSheet(
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
                    await ShowErrorAsync("Camera is not supported on this device.");
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
            await ShowErrorAsync($"An error occurred: {ex.Message}");
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
            var itemDto = await _completedPhotoRepository.UploadPhotoAsync(
                localFilePath,
                CustomerJobAndOccurrenceIds.CustomerId,
                CustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
                CustomerJobAndOccurrenceIds.JobOccurrenceId);

            if (itemDto.IsSuccess)
            {
                var photoDetails = itemDto.Value.CompletedPhotoUploadDto;
                await ShowSuccessAsync("Photo Uploaded", "The photo has been successfully uploaded.");
                PhotoPaths.Add(new PhotoDisplayItem(photoDetails.JobCompletedPhotoId, photoDetails.FilePath));
            }
            else
            {
                await ShowErrorAsync("Upload Failed - Something went wrong uploading the photo.");
            }
        });
    }

    [RelayCommand]
    private async Task RemovePhotoAsync(PhotoDisplayItem photo)
    {
        if (CustomerJobAndOccurrenceIds is null)
            return;

        await RunWithSpinner(async () =>
        {
            var photoId = photo.Id!.Value;
            var response = await _completedPhotoRepository.DeletePhotoAsync(
                CustomerJobAndOccurrenceIds.CustomerId,
                CustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
                CustomerJobAndOccurrenceIds.JobOccurrenceId,
                photoId);

            if (!response.IsSuccess)
            {
                await ShowErrorAsync("Warning - Failed to delete photo from server.");
                return;
            }

            PhotoPaths.Remove(photo);
            OnPropertyChanged(nameof(PhotoPaths));
        });
    }
}