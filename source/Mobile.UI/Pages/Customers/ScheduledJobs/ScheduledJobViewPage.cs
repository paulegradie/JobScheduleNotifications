using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

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
                Spacing = 20,

                Children =
                {
                    new Label().Bind(Label.TextProperty, nameof(ViewModel.Title))
                        .FontSize(24).Bold(),

                    new Label().Text("Customer:")
                        .FontSize(14),
                    new Label().Bind(Label.TextProperty, nameof(ViewModel.CustomerName))
                        .FontSize(14),

                    new Label().Text("Anchor Date:")
                        .FontSize(14),
                    new Label()
                        .Bind(Label.TextProperty,
                            nameof(ViewModel.AnchorDate),
                            stringFormat: "{0:MM/dd/yyyy h:mm tt}")
                        .FontSize(14),

                    new Label().Text("Description:")
                        .FontSize(14),
                    new Label().Bind(Label.TextProperty, nameof(ViewModel.Description))
                        .FontSize(14),

                    new Label().Bind(Label.TextProperty, nameof(ViewModel.CronExpression),
                            stringFormat: "Cron: {0}")
                        .FontSize(14),

                    new Button()
                        .Text("Add Occurence")
                        .Bind(Button.CommandProperty, nameof(ViewModel.AddOccurrenceCommand), source: vm)
                        .Bind(Button.CommandParameterProperty, nameof(vm.Details), source: this)
                        .Row(Row.ViewOccurrences)
                        .RowSpan(3).Column(1)
                        .CenterVertical(),


                    // ——— Our new section ———
                    new Label().Text("Job Occurrences:")
                        .FontSize(18).Bold(),

                    // The list of occurrences
                    new CollectionView
                        {
                            ItemsLayout = LinearItemsLayout.Vertical,
                            SelectionMode = SelectionMode.None,
                            EmptyView = "No occurrences yet",
                            Background = Colors.Brown,
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

                                var grid = new Grid
                                {
                                    Margin = 2,
                                    RowDefinitions = Rows.Define(
                                        (Row.Date, Auto),
                                        (Row.Status, Auto),
                                        (Row.ViewOccurrences, Auto) // ← add this line
                                    ),
                                    ColumnDefinitions = Columns.Define(
                                        (Col.Main, Star)
                                    )
                                };
                                grid.Add(new Button()
                                    .Text("View")
                                    .Bind(Button.CommandProperty,
                                        nameof(vm.NavigateToOccurrenceCommand),
                                        source: vm)
                                    // bind CommandParameter to the item’s JobOccurrenceId
                                    .Bind(Button.CommandParameterProperty,
                                        nameof(JobOccurrenceDto.JobOccurrenceId))
                                    .Row(Row.ViewOccurrences)
                                    .Column(Col.Main)
                                );

                                frame.Content = grid;
                                return frame;
                            })
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(vm.JobOccurrences))
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

    private enum Row
    {
        Date,
        Status,
        ViewOccurrences
    }

    private enum Col
    {
        Main
    }
}