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
                    new Label().Bind(Label.TextProperty, nameof(ViewModel.AnchorDate),
                            stringFormat: "{0:MM/dd/yyyy}")
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
                        .Bind(Button.CommandParameterProperty, nameof(Details))
                        .Row(Row.ViewOccurrences)
                        .RowSpan(3).Column(1)
                        .CenterVertical(),


                    // ——— Our new section ———
                    new Label().Text("Occurrences:")
                        .FontSize(18).Bold(),

                    // The list of occurrences
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

                                var grid = new Grid
                                {
                                    RowDefinitions = Rows.Define((Row.Date, Auto), (Row.Status, Auto)),
                                    ColumnDefinitions = Columns.Define((Col.Main, Star))
                                };

                                grid.Add(new Label()
                                        .Font(size: 16, bold: true)
                                        .Bind(Label.TextProperty, nameof(JobOccurrenceDto.OccurrenceDate),
                                            stringFormat: "{0:MMM d, yyyy h:mm tt}"),
                                    (int)Row.Date, (int)Col.Main);

                                grid.Add(new Label()
                                        .Font(size: 14)
                                        .Bind(
                                            Label.TextProperty,
                                            nameof(JobOccurrenceDto.CompletedDate),
                                            // format when non-null
                                            stringFormat: "Status: {0}",
                                            // what to show when the source is null
                                            fallbackValue: "Status: null"
                                        ),
                                    (int)Row.Status, (int)Col.Main);

                                frame.Content = grid;

                                frame.GestureRecognizers.Add(new TapGestureRecognizer()
                                    .Bind(TapGestureRecognizer.CommandProperty, nameof(vm.NavigateToOccurrencesCommand))
                                    .Bind(TapGestureRecognizer.CommandParameterProperty, nameof(JobOccurrenceDto.JobOccurrenceId)));

                                return frame;
                            })
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(vm.Occurrences))
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