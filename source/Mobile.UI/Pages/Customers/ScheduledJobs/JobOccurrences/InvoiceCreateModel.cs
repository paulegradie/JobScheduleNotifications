using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public record InvoiceItem(string ItemNumber, string Description, decimal Price);

public partial class InvoiceCreateModel : BaseViewModel
{
    private readonly INavigationRepository _navigationRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IJobCompletedPhotoRepository _photoRepository;
    [ObservableProperty] private string _currentItemDescription = string.Empty;
    [ObservableProperty] private string _currentItemPrice = string.Empty;
    [ObservableProperty] private string _today = DateTime.Now.ToString("yyyy-MM-dd");

    [ObservableProperty] private string _total;
    public ObservableCollection<InvoiceItem> InvoiceItems { get; } = new();

    [ObservableProperty] private string? _previewFilePath;

    private string? _jobDescription;

    private CustomerJobAndOccurrenceIds CusterCustomerJobAndOccurrenceIds { get; set; }

    public InvoiceCreateModel(INavigationRepository navigationRepository, IInvoiceRepository invoiceRepository, IJobCompletedPhotoRepository photoRepository)
    {
        _navigationRepository = navigationRepository;
        _invoiceRepository = invoiceRepository;
        _photoRepository = photoRepository;
    }

    public void Initialize(string customerId, string scheduledJobDefinitionId, string jobOccurrenceId, string jobDescription)
    {
        _jobDescription = jobDescription;
        Total = InvoiceItems.Sum(x => x.Price).ToString("C");
        CusterCustomerJobAndOccurrenceIds = new CustomerJobAndOccurrenceIds(
            new CustomerId(Guid.Parse(customerId)),
            new ScheduledJobDefinitionId(Guid.Parse(scheduledJobDefinitionId)),
            new JobOccurrenceId(Guid.Parse(jobOccurrenceId)));
        OnPropertyChanged(nameof(InvoiceItems));
        OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    private void AddItem()
    {
        if (string.IsNullOrWhiteSpace(CurrentItemDescription) ||
            !int.TryParse(CurrentItemPrice, out var price)) return;
        InvoiceItems.Add(new InvoiceItem((InvoiceItems.Count + 1).ToString(), CurrentItemDescription, price));
        CurrentItemDescription = string.Empty;
        CurrentItemPrice = string.Empty;
        Total = InvoiceItems.Sum(x => x.Price).ToString("C");

        OnPropertyChanged(nameof(InvoiceItems));
        OnPropertyChanged(nameof(CurrentItemDescription));
        OnPropertyChanged(nameof(CurrentItemPrice));
        OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    private async Task GeneratePdfAsync()
    {
        var outputPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"Invoice_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        PreviewFilePath = outputPath;

        QuestPDF.Settings.License = LicenseType.Community;

        var invoiceNumber = $"INV-20240614-{Guid.NewGuid().ToString("N").Split("-")[0]}"; // You can auto-generate
        var customerName = "John Doe"; // Supply from your model
        var invoiceDate = DateTime.Now;

        var subtotal = InvoiceItems.Sum(item => item.Price);
        var total = subtotal; // Add tax if needed

        var photoPathsResult = await _photoRepository.ListPhotoAsync(
            CusterCustomerJobAndOccurrenceIds.CustomerId,
            CusterCustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
            CusterCustomerJobAndOccurrenceIds.JobOccurrenceId
        );

        var photoUris = new List<string>();
        if (photoPathsResult.IsSuccess)
        {
            photoUris.AddRange(photoPathsResult.Value.JobCompletedPhotoListDto.JobCompletedPhotos.Select(details => details.Uri));
        }

        var location = new InvoiceDocument.CustomerBusinessLocation("123 Main Street", "Melbourne", "Victoria", "Australia");
        var businessDetails = new InvoiceDocument.CustomerBusinessDetails("Your Business Name", "1234567890", "info@yourbusiness.com", location, "1234567890");

        var document = new InvoiceDocument(businessDetails, invoiceNumber, customerName, invoiceDate, InvoiceItems, "bank-details-123", photoUris);
        document.GeneratePdf(outputPath);

        OnPropertyChanged(nameof(PreviewFilePath));

        await _invoiceRepository.SendInvoiceAsync(
            outputPath,
            CusterCustomerJobAndOccurrenceIds.CustomerId,
            CusterCustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
            CusterCustomerJobAndOccurrenceIds.JobOccurrenceId);
        await _navigationRepository.ShowAlertAsync("Success", $"Invoice saved to:\n{outputPath}");
    }
}