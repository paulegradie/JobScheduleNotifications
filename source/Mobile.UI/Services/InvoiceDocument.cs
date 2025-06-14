﻿using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using IContainer = QuestPDF.Infrastructure.IContainer;

public class InvoiceDocument : IDocument
{
    private readonly CustomerBusinessDetails _customerBusinessDetails;
    public string InvoiceNumber { get; }
    public string CustomerName { get; }
    public DateTime InvoiceDate { get; }
    public IReadOnlyCollection<InvoiceItem> InvoiceItems { get; }
    public string BankDetails { get; }
    public IReadOnlyCollection<string> PhotoFilePaths { get; }

    public InvoiceDocument(
        CustomerBusinessDetails customerBusinessDetails,
        string invoiceNumber,
        string customerName,
        DateTime invoiceDate,
        IReadOnlyCollection<InvoiceItem> invoiceItems,
        string bankDetails,
        IReadOnlyCollection<string> photoFilePaths)
    {
        _customerBusinessDetails = customerBusinessDetails;
        InvoiceNumber = invoiceNumber;
        CustomerName = customerName;
        InvoiceDate = invoiceDate;
        InvoiceItems = invoiceItems;
        BankDetails = bankDetails;
        PhotoFilePaths = photoFilePaths;
    }

    public record CustomerBusinessDetails(
        string BusinessName,
        string PhoneNumber,
        string Email,
        CustomerBusinessLocation? CustomerBusinessLocation = null,
        string? BusinessIdLikeAbn = null);

    public record CustomerBusinessLocation(string Address = "123 Main Street", string City = "Melbourne", string State = "Victoria", string Country = "Australia");

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Row(header =>
            {
                header.RelativeItem().Column(col =>
                {
                    col.Item().Text(_customerBusinessDetails.BusinessName).FontSize(20).Bold();

                    if (_customerBusinessDetails.CustomerBusinessLocation is not null)
                    {
                        col.Item().Text(_customerBusinessDetails.CustomerBusinessLocation.Address);
                        col.Item().Text(
                            $"{_customerBusinessDetails.CustomerBusinessLocation.City}, {_customerBusinessDetails.CustomerBusinessLocation.State}, {_customerBusinessDetails.CustomerBusinessLocation.Country}");
                    }

                    col.Item().Text($"Phone: {_customerBusinessDetails.PhoneNumber}");
                    col.Item().Text($"Email: {_customerBusinessDetails.Email}");
                    if (!string.IsNullOrWhiteSpace(_customerBusinessDetails.BusinessIdLikeAbn))
                    {
                        col.Item().Text($"ABN: {_customerBusinessDetails.BusinessIdLikeAbn}");
                    }
                });

                header.ConstantItem(200).Column(col =>
                {
                    col.Item().Text($"Invoice #: {Guid.NewGuid():X}").FontSize(12);
                    col.Item().Text($"Date: {InvoiceDate:yyyy-MM-dd}");
                    col.Item().Text($"Customer: {CustomerName}");
                });
            });

            page.Content().PaddingVertical(20).Column(content =>
            {
                content.Item().PaddingBottom(10).LineHorizontal(1); //;

                ComposeTable(content);

                content.Item().PaddingVertical(20).LineHorizontal(1);

                ComposeTotals(content);

                content.Item().PaddingTop(40).Column(footer =>
                {
                    footer.Item().Text("Bank Transfer Details:").SemiBold();
                    footer.Item().Text(BankDetails);
                });

                content.Item().PaddingTop(20).LineHorizontal(1);

                ComposePhotos(content);
            });

            page.Footer().AlignCenter().Text("Thank you for your business!").FontSize(10).Italic();
        });
    }

    private void ComposeTable(ColumnDescriptor content)
    {
        content.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50); // Item #
                columns.RelativeColumn(3); // Description
                columns.ConstantColumn(80); // Price
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Item");
                header.Cell().Element(CellStyle).Text("Description");
                header.Cell().Element(CellStyle).AlignRight().Text("Price");

                static IContainer CellStyle(IContainer container) =>
                    container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).PaddingBottom(5);
            });

            foreach (var item in InvoiceItems)
            {
                table.Cell().Text(item.ItemNumber);
                table.Cell().Text(item.Description);
                table.Cell().AlignRight().Text($"{item.Price:C}");
            }
        });
    }

    private void ComposeTotals(ColumnDescriptor content)
    {
        var subtotal = InvoiceItems.Sum(x => x.Price);
        var tax = 0m; // Can extend
        var total = subtotal + tax;

        content.Item().AlignRight().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.ConstantColumn(100);
            });

            // table.Cell().Text("Subtotal:");
            // table.Cell().AlignRight().Text($"{subtotal:C}");

            table.Cell().Text("Total:");
            table.Cell().AlignRight().Text($"{total:C}");
        });
    }

    private void ComposePhotos(ColumnDescriptor content)
    {
        if (PhotoFilePaths == null || PhotoFilePaths.Count == 0)
            return;

        content.Item().PaddingBottom(10).Text("Attached Photos:").SemiBold().FontSize(14);

        // Group photos into pairs for 2-column layout
        var photoList = PhotoFilePaths.Where(File.Exists).ToList();

        for (int i = 0; i < photoList.Count; i += 2)
        {
            content.Item().PaddingBottom(10).Row(row =>
            {
                // First photo in the row
                row.RelativeItem().PaddingRight(5).AspectRatio(1.5f).Image(photoList[i]).FitArea();

                // Second photo in the row (if exists)
                if (i + 1 < photoList.Count)
                {
                    row.RelativeItem().PaddingLeft(5).AspectRatio(1.5f).Image(photoList[i + 1]).FitArea();
                }
                else
                {
                    // Empty space if odd number of photos
                    row.RelativeItem();
                }
            });
        }

        // Handle any photos that couldn't be loaded
        var missingPhotos = PhotoFilePaths.Where(path => !File.Exists(path)).ToList();
        foreach (var path in missingPhotos)
        {
            content.Item().Text($"[Could not load image: {path}]").Italic();
        }
    }
}