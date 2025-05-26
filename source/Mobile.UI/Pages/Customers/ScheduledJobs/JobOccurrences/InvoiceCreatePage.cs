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

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
                Children =
                {
                    new Entry().Placeholder("Item Description")
                        .Bind(Entry.TextProperty, nameof(vm.CurrentItemDescription)),

                    new Entry
                        {
                            Placeholder = "Price ($)",
                            Keyboard = Keyboard.Numeric
                        }
                        .Bind(Entry.TextProperty, nameof(vm.CurrentItemPrice)),

                    new Button()
                        .Text("Add Item")
                        .Bind(Button.CommandProperty, nameof(vm.AddItemCommand)),

                    new CollectionView
                    {
                        ItemsSource = vm.InvoiceItems,
                        ItemTemplate = new DataTemplate(() =>
                            new Label().Bind(Label.TextProperty, ".")
                        )
                    },

                    new Button()
                        .Text("Generate PDF")
                        .Bind(Button.CommandProperty, nameof(vm.GeneratePdfCommand))
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