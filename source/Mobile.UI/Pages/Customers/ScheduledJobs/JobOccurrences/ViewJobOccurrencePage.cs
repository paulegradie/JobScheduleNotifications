using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

[QueryProperty(nameof(JobOccurrenceId), "JobOccurrenceId")]
[QueryProperty(nameof(ScheduledJobDefinitionId), "ScheduledJobDefinitionId")]
[QueryProperty(nameof(CustomerId), "CustomerId")]
public class ViewJobOccurrencePage : BasePage<ViewJobOccurrenceModel>
{
    public string JobOccurrenceId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    public ViewJobOccurrencePage(ViewJobOccurrenceModel vm) : base(vm)
    {
        Title = "Occurrence Detail";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
                Children =
                {
                    // Job title
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.JobTitle))
                        .FontSize(24).Bold(),

                    // Occurrence date
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.OccurrenceDate),
                            stringFormat: "Occurred: {0:MMM d, yyyy h:mm tt}")
                        .FontSize(16),

                    // Completed date (prints “null” if not set)
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.CompletedDate),
                            stringFormat: "Completed: {0:MMM d, yyyy h:mm tt}",
                            fallbackValue: "Completed: null")
                        .FontSize(16)
                        .Bind(IsVisibleProperty, nameof(vm.MarkedAsComplete)),

                    // Mark as Completed
                    new Button()
                        .Text("Mark as Completed")
                        .Bind(IsVisibleProperty, nameof(ViewModel.CanMarkComplete))
                        .Bind(Button.CommandProperty, nameof(ViewModel.MarkCompletedCommand)),

                    // Upload Photo
                    new Button()
                        .Text("Upload Photo")
                        .IsEnabled(true)
                        .Bind(Button.CommandProperty, nameof(ViewModel.UploadPhotoCommand)),

                    // Create Invoice
                    new Button()
                        .Text("Create Invoice")
                        .Bind(Button.CommandProperty, nameof(ViewModel.CreateInvoiceCommand)),
                    new Button()
                        .Text("Go Back")
                        .IsEnabled(true)
                        .Bind(Button.CommandProperty, nameof(ViewModel.GoBackCommand)),

                    new CollectionView
                        {
                            ItemTemplate = new DataTemplate(() =>
                            {
                                var image = new Image()
                                    .Height(150)
                                    .Aspect(Aspect.AspectFill);
                                image.Bind(Image.SourceProperty, nameof(PhotoDisplayItemDto.Path));

                                var removeButton = new Button()
                                    .Text("Remove")
                                    .Bind(Button.CommandParameterProperty, ".")
                                    .Bind(Button.CommandProperty, nameof(ViewModel.RemovePhotoCommand));

                                return new VerticalStackLayout
                                {
                                    Children = { image, removeButton }
                                };
                            })
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.PhotoPaths))
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadCommand.Execute(new CustomerJobAndOccurrenceIds(
            new CustomerId(Guid.Parse(CustomerId)),
            new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId)),
            new JobOccurrenceId(Guid.Parse(JobOccurrenceId))));
    }
}