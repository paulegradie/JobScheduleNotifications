using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

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
                    Section("Title",
                        new Label().Bind(Label.TextProperty, nameof(vm.Title))
                            .FontSize(24).Bold()),

                    Section("Customer",
                        new Label().Bind(Label.TextProperty, nameof(vm.CustomerName))
                            .FontSize(14)),

                    Section("Anchor Date",
                        new Label().Bind(Label.TextProperty, nameof(vm.AnchorDate), stringFormat: "{0:MM/dd/yyyy h:mm tt}")
                            .FontSize(14)),

                    Section("Description",
                        new Label().Bind(Label.TextProperty, nameof(vm.Description))
                            .FontSize(14)),

                    Section("Cron Expression",
                        new Label().Bind(Label.TextProperty, nameof(vm.CronExpression), stringFormat: "Cron: {0}")
                            .FontSize(14)),

                    new Button { Text = "Add Occurrence", CornerRadius = 8 }
                        .BindCommand(nameof(vm.AddOccurrenceCommand), parameterSource: vm.Details),
                    // .Bind(IsEnabledProperty, nameof(vm.CanAddOccurrence)),

                    Section("Job Occurrences",
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

                                    // Layout for each occurrence
                                    var stack = new VerticalStackLayout { Spacing = 6 };

                                    // Occurrence date
                                    stack.Children.Add(
                                        new Label()
                                            .Bind(Label.TextProperty, nameof(JobOccurrenceDto.OccurrenceDate), stringFormat: "{0:MMM d, yyyy h:mm tt}")
                                            .FontSize(14).TextColor(Colors.Black)
                                    );
                                    // Job title
                                    stack.Children.Add(
                                        new Label()
                                            .Bind(Label.TextProperty, nameof(JobOccurrenceDto.JobTitle))
                                            .FontSize(14).TextColor(Colors.Black)
                                    );
                                    // Completion status
                                    stack.Children.Add(
                                        new Label()
                                            .Bind(Label.TextProperty, nameof(JobOccurrenceDto.MarkedAsCompleted), stringFormat: "Completed: {0}")
                                            .FontSize(14).TextColor(Colors.Black)
                                    );
                                    // // View button
                                    // stack.Children.Add(
                                    //     new Button { Text = "View" }
                                    //         .BindCommand(nameof(vm.NavigateToOccurrenceCommand), parameterSource: new Binding(nameof(JobOccurrenceDto.JobOccurrenceId)))
                                    // );

                                    // View button
                                    stack.Children.Add(
                                        new Button()
                                            .Text("View")
                                            .Bind(Button.CommandProperty, nameof(vm.NavigateToOccurrenceCommand), source: vm)
                                            // bind CommandParameter to the item’s JobOccurrenceId
                                            .Bind(Button.CommandParameterProperty, nameof(JobOccurrenceDto.JobOccurrenceId)));
                                    frame.Content = stack;
                                    return frame;
                                })
                            }
                            .Bind(ItemsView.ItemsSourceProperty, nameof(vm.JobOccurrences))
                    ),

                    Section("Reminders Sent",
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
                                            .Bind(Button.TextProperty, nameof(JobReminderDto.ReminderDateTime), stringFormat: "{0:MMM d, yyyy h:mm tt}")
                                            .BindCommand(nameof(ViewModel.NavigateToReminderCommand), parameterSource: new Binding(nameof(JobReminderDto.JobReminderId)))
                                    }
                                )
                            }
                            .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.JobReminders))
                    )
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