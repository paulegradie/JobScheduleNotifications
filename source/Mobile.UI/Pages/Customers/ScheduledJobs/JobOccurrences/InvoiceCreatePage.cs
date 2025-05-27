using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(ScheduledJobDefinitionId), "ScheduledJobDefinitionId")]
[QueryProperty(nameof(JobOccurrenceId), "JobOccurrenceId")]
[QueryProperty(nameof(JobDescription), "JobDescription")]
public sealed class InvoiceCreatePage : BasePage<InvoiceCreateModel>
{
    public string CustomerId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }
    public string JobOccurrenceId { get; set; }
    public string JobDescription { get; set; }

    public InvoiceCreatePage(InvoiceCreateModel vm) : base(vm)
    {
        Title = "Create Invoice";

        Content = Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    new Label()
                        .Text("Invoice Date:")
                        .FontSize(12)
                        .TextColor(Colors.Gray),

                    new Label()
                        .FontSize(14)
                        .Bind(Label.TextProperty, nameof(ViewModel.Today)),

                    new Entry()
                        .Placeholder("Item Description")
                        .Bind(Entry.TextProperty, nameof(ViewModel.CurrentItemDescription)),

                    new Entry
                        {
                            Placeholder = "Price ($)",
                            Keyboard = Keyboard.Numeric
                        }
                        .Bind(Entry.TextProperty, nameof(ViewModel.CurrentItemPrice)),

                    new Button()
                        .Text("Add Item")
                        .Bind(Button.CommandProperty, nameof(ViewModel.AddItemCommand)),

                    new CollectionView
                        {
                            EmptyView = "No items to invoice",

                            ItemTemplate = new DataTemplate(() =>
                                new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Label()
                                            .FontSize(14).TextColor(Colors.Black)
                                            .Bind(Label.TextProperty, "Description")
                                            .CenterVertical(),

                                        new Label()
                                            .FontSize(14).TextColor(Colors.Black)
                                            .Bind(Label.TextProperty, "Price", stringFormat: "${0:F2}")
                                            .CenterVertical()
                                    }
                                }
                            )
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.InvoiceItems)),

                    new Label()
                        .FontSize(16)
                        .Bold()
                        .TextColor(Colors.DarkGreen)
                        .Bind(Label.TextProperty, nameof(ViewModel.Total), stringFormat: "Total: {0:F2}"),

                    new Button()
                        .Text("Generate PDF")
                        .Bind(Button.CommandProperty, nameof(ViewModel.GeneratePdfCommand)),
                    new Grid
                    {
                        Children =
                        {
                            new Label()
                                .Text("No preview yet")
                                .FontSize(12)
                                .TextColor(Colors.Gray)
                                .Bind(IsVisibleProperty, nameof(ViewModel.PreviewFilePath), convert: (string? path) => string.IsNullOrWhiteSpace(path)),

                            new WebView
                                {
                                    HeightRequest = 500,
                                    WidthRequest = 350
                                }
                                .Bind(IsVisibleProperty, nameof(ViewModel.PreviewFilePath), convert: (string? path) => !string.IsNullOrWhiteSpace(path))
                                .Bind(WebView.SourceProperty, nameof(ViewModel.PreviewFilePath), convert: (string? path) =>
                                {
#if ANDROID
                return $"file://{path}";
#elif WINDOWS
                return path;
#elif IOS
                return new Foundation.NSUrl(path, false).AbsoluteString;
#else
                                    return path;
#endif
                                })
                        }
                    }
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.Initialize(CustomerId, ScheduledJobDefinitionId, JobOccurrenceId, JobDescription);
    }
}