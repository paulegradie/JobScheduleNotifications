// using QuestPDF.Fluent;
// using QuestPDF.Helpers;
// using QuestPDF.Infrastructure;
//
// QuestPDF.Settings.License = LicenseType.Community; // required
//
// Document.Create(container =>
// {
//     container.Page(page =>
//     {
//         page.Margin(50);
//         page.Header().Text("Invoice").FontSize(20).Bold();
//         page.Content().Column(col =>
//         {
//             col.Item().Text("Customer: Jane Doe");
//             col.Item().Text("Date: 2025-05-26");
//             col.Item().LineHorizontal(1);
//             col.Item().Text("Item 1 - $100");
//             col.Item().Text("Item 2 - $250");
//             col.Item().LineHorizontal(1);
//             col.Item().Text("Total: $350").Bold();
//         });
//     });
// }).GeneratePdf("invoice.pdf");