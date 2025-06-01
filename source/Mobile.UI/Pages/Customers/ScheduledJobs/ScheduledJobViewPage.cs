using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

/// <summary>
/// Page for viewing a scheduled job, its occurrences, and reminders, with paging support.
/// </summary>
[QueryProperty(nameof(ScheduledJobDefinitionId), "ScheduledJobDefinitionId")]
[QueryProperty(nameof(CustomerId), "CustomerId")]
public sealed class ScheduledJobViewPage : BasePage<ScheduledJobViewModel>
{
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    public ScheduledJobViewPage(ScheduledJobViewModel vm) : base(vm)
    {
        Title = "View Scheduled Job";
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 25,
                Children =
                {
                    Section("Title", new Label()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.Title))
                        .FontSize(24).Bold()),

                    Section("Customer", new Label()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.CustomerName))
                        .FontSize(14)),

                    Section("Anchor Date", new Label()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.AnchorDate), stringFormat: "{0:MM/dd/yyyy h:mm tt}")
                        .FontSize(14)),

                    Section("Description", new Label()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.Description))
                        .FontSize(14)),

                    Section("Cron Expression", new Label()
                        .Bind(Label.TextProperty, nameof(ScheduledJobViewModel.CronExpression), stringFormat: "Cron: {0}")
                        .FontSize(14)),

                    new Button { Text = "Add Occurrence", CornerRadius = 8 }
                        .BindCommand(nameof(ScheduledJobViewModel.AddOccurrenceCommand), source: vm),

                    Section("Job Occurrences",
                        new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new CollectionView
                                    {
                                        ItemsLayout = LinearItemsLayout.Vertical,
                                        SelectionMode = SelectionMode.None,
                                        EmptyView = "No occurrences yet",
                                        ItemTemplate = new DataTemplate(() =>
                                        {
                                            var frame = new Frame
                                            {
                                                Padding = 10,
                                                CornerRadius = 6,
                                                HasShadow = false,
                                                BorderColor = Colors.LightGray,
                                                BackgroundColor = Colors.White,
                                                Margin = new Thickness(0, 5)
                                            };

                                            var stack = new VerticalStackLayout { Spacing = 6 };
                                            stack.Children.Add(new Label()
                                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.OccurrenceDate), stringFormat: "{0:MMM d, yyyy h:mm tt}")
                                                .FontSize(14).TextColor(Colors.Black));
                                            stack.Children.Add(new Label()
                                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.JobTitle))
                                                .FontSize(14).TextColor(Colors.Black));
                                            stack.Children.Add(new Label()
                                                .Bind(Label.TextProperty, nameof(JobOccurrenceDto.MarkedAsCompleted), stringFormat: "Completed: {0}")
                                                .FontSize(14).TextColor(Colors.Black));
                                            stack.Children.Add(new Button()
                                                .Text("View")
                                                .Bind(Button.CommandProperty, nameof(ScheduledJobViewModel.NavigateToOccurrenceCommand), source: vm)
                                                .Bind(Button.CommandParameterProperty, nameof(JobOccurrenceDto.JobOccurrenceId)));
                                            frame.Content = stack;
                                            return frame;
                                        })
                                    }
                                    .Bind(ItemsView.ItemsSourceProperty, nameof(ScheduledJobViewModel.JobOccurrences)),

                                new Button()
                                    .Text("Load More")
                                    .Bind(IsVisibleProperty, nameof(ScheduledJobViewModel.HasMoreOccurrences))
                                    .Bind(Button.CommandProperty, nameof(ScheduledJobViewModel.LoadMoreOccurrencesCommand)),
                            }
                        })
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadScheduledJobCommand.Execute(
            new Details(
                new CustomerId(Guid.Parse(CustomerId)),
                new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId))
            ));
    }

    private static VerticalStackLayout Section(string label, View control) =>
        new()
        {
            Spacing = 4,
            Children =
            {
                new Label().Text(label).Font(size: 14, bold: true),
                control
            }
        };
}