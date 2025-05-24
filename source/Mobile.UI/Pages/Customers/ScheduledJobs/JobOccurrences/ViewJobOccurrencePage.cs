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
                            stringFormat: "Completed: {0:MMM d, yyyy h:mm tt} -- selected",
                            fallbackValue: "Completed: null")
                        .FontSize(16)
                        .Bind(IsVisibleProperty, nameof(vm.MarkedAsComplete)),

                    // Mark as Completed
                    new Button()
                        .Text("Mark as Completed")
                        .Bind(Button.IsVisibleProperty, nameof(ViewModel.CanMarkComplete))
                        .Bind(Button.CommandProperty, nameof(ViewModel.MarkCompletedCommand)),

                    // Upload Photo stub
                    new Button()
                        .Text("Upload Photo")
                        .IsEnabled(false),

                    // Create Invoice stub
                    new Button()
                        .Text("Create Invoice")
                        .IsEnabled(false),
                    new Label().Text("Reminders").FontSize(20).Bold(),
                    new CollectionView
                        {
                            SelectionMode = SelectionMode.Single,
                            EmptyView = "No reminders",
                            ItemTemplate = new DataTemplate(() =>
                                new Frame
                                {
                                    Padding = 10,
                                    CornerRadius = 6,
                                    BackgroundColor = Colors.LightGray,
                                    HasShadow = true,
                                    Content = new Button()
                                        .Bind(Button.TextProperty, nameof(JobReminderDto.ReminderDate), stringFormat: "{0:MMM d, yyyy h:mm tt}")
                                        .Bind(Button.CommandProperty, nameof(ViewModel.NavigateToReminderCommand))
                                        .Bind(Button.CommandParameterProperty, nameof(JobReminderDto.JobReminderId))
                                }
                            )
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.JobReminderDtos)),

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