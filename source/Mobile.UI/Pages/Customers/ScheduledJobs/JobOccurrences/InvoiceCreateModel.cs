﻿using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public partial class InvoiceCreateModel : BaseViewModel
{
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

    public InvoiceCreateModel(
        IInvoiceRepository invoiceRepository,
        IJobCompletedPhotoRepository photoRepository)
    {
        _invoiceRepository = invoiceRepository;
        _photoRepository = photoRepository;
    }

    public async Task Initialize(string customerId, string scheduledJobDefinitionId, string jobOccurrenceId)
    {
        _jobDescription = string.Empty;
        Total = InvoiceItems.Sum(x => x.Price).ToString("C");
        CusterCustomerJobAndOccurrenceIds = new CustomerJobAndOccurrenceIds(
            CustomerId.Parse(customerId),
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
        var localOutputPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"Invoice_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        PreviewFilePath = localOutputPath;

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

        try
        {
            document.GeneratePdf(localOutputPath);

            OnPropertyChanged(nameof(PreviewFilePath));

            var result = await _invoiceRepository.SaveInvoiceAsync(
                localOutputPath,
                InvoiceItems,
                CusterCustomerJobAndOccurrenceIds.CustomerId,
                CusterCustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
                CusterCustomerJobAndOccurrenceIds.JobOccurrenceId
            );

            if (result.IsSuccess)
            {
                await ShowSuccessAsync($"Invoice saved to:\n{localOutputPath}");
            }
            else
            {
                await ShowErrorAsync("Failed to save invoice.", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Failed", ex.Message);
        }
    }

    [RelayCommand]
    private async Task SendInvoice()
    {
        await RunWithSpinner(async () =>
        {
            if (PreviewFilePath == null) return;

            var result = await _invoiceRepository.SendInvoiceAsync(
                PreviewFilePath,
                CusterCustomerJobAndOccurrenceIds.CustomerId,
                CusterCustomerJobAndOccurrenceIds.ScheduledJobDefinitionId,
                CusterCustomerJobAndOccurrenceIds.JobOccurrenceId
            );

            if (result.IsSuccess)
            {
                await ShowSuccessAsync("Invoice sent!");
            }
            else
            {
                await ShowErrorAsync($"Failed to send invoice: {result.ErrorMessage}");
            }
        });
    }
}