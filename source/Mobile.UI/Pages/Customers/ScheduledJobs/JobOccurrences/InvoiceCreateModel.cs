using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public record InvoiceItem(string Description, decimal Price);

public partial class InvoiceCreateModel : BaseViewModel
{
    private readonly INavigationRepository _navigationRepository;
    [ObservableProperty] private string _currentItemDescription = string.Empty;
    [ObservableProperty] private string _currentItemPrice = string.Empty;
    [ObservableProperty] private string _today = DateTime.Now.ToString("yyyy-MM-dd");

    [ObservableProperty] private string _total;
    public ObservableCollection<InvoiceItem> InvoiceItems { get; } = new();
    
    [ObservableProperty]
    private string? _previewFilePath;
    
    private string? _jobDescription;

    public InvoiceCreateModel(INavigationRepository navigationRepository)
    {
        _navigationRepository = navigationRepository;
    }

    public void Initialize(string customerId, string scheduledJobDefinitionId, string jobOccurrenceId, string jobDescription)
    {
        _jobDescription = jobDescription;
        Total = InvoiceItems.Sum(x => x.Price).ToString("C");
        OnPropertyChanged(nameof(InvoiceItems));
        OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    private void AddItem()
    {
        if (string.IsNullOrWhiteSpace(CurrentItemDescription) ||
            !int.TryParse(CurrentItemPrice, out var price)) return;
        InvoiceItems.Add(new InvoiceItem(CurrentItemDescription, price));
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

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("Invoice").FontSize(24).Bold();

                page.Content().Column(col =>
                {
                    col.Item().Text($"Date: {DateTime.Now:yyyy-MM-dd}");
                    col.Item().LineHorizontal(1);
                    foreach (var item in InvoiceItems)
                        col.Item().Text(item);
                    col.Item().LineHorizontal(1);
                });
            });
        }).GeneratePdf(outputPath);
        OnPropertyChanged(nameof(PreviewFilePath));

        await _navigationRepository.ShowAlertAsync("Success", $"Invoice saved to:\n{outputPath}");
    }
}