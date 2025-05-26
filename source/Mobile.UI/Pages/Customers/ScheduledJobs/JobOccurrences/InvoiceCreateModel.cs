using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public partial class InvoiceCreateModel : BaseViewModel
{
    private readonly INavigationRepository _navigationRepository;
    [ObservableProperty] private string currentItemDescription = string.Empty;
    [ObservableProperty] private string currentItemPrice = string.Empty;

    public ObservableCollection<string> InvoiceItems { get; } = new();

    private string? _jobDescription;

    public InvoiceCreateModel(INavigationRepository navigationRepository)
    {
        _navigationRepository = navigationRepository;
    }
    
    public void Initialize(string customerId, string scheduledJobDefinitionId, string jobOccurrenceId, string jobDescription)
    {
        _jobDescription = jobDescription;
        InvoiceItems.Add($"Job Description: {_jobDescription}");
    }

    [RelayCommand]
    private void AddItem()
    {
        if (!string.IsNullOrWhiteSpace(CurrentItemDescription) &&
            int.TryParse(CurrentItemPrice, out var price))
        {
            InvoiceItems.Add($"{CurrentItemDescription} - ${price}");
            CurrentItemDescription = string.Empty;
            CurrentItemPrice = string.Empty;
        }
    }

    [RelayCommand]
    private async Task GeneratePdfAsync()
    {
        var outputPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"Invoice_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

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

        await _navigationRepository.ShowAlertAsync("Success", $"Invoice saved to:\n{outputPath}");
    }
}