using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(ScheduledJobDefinitionId), "scheduledJobDefinitionId")]
public sealed class ScheduledJobViewPage : BasePage<ScheduledJobViewModel>
{
    public string ScheduledJobDefinitionId { get; set; }


    public ScheduledJobViewPage(ScheduledJobViewModel vm) : base(vm)
    {
        Title = "View Details for a Scheduled Job:";

        // Define UI layout
        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = 20,
                RowDefinitions = Rows.Define(
                    (Row.Title, Auto),
                    (Row.Info, Auto),
                    (Row.OccurrencesLabel, Auto),
                    (Row.OccurrencesList, Star)
                ),
                ColumnDefinitions = Columns.Define((Column.One, Star)),
                Children =
                {
                    // Title
                    new Label()
                        .Text("Job Title:")
                        .Bind(Label.TextProperty, nameof(ViewModel.Title))
                        .FontSize(24)
                        .Bold()
                        .Row(Row.Title),

                    // Customer Name
                    new Label()
                        .Text("Customer:")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One),
                    new Label()
                        .Text("Customer Name")
                        .Bind(Label.TextProperty, nameof(ViewModel.CustomerName))
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(80, 0, 0, 0)),

                    // Anchor Date
                    new Label()
                        .Text("Anchor Date:")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(0, 30, 0, 0)),
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.AnchorDate), BindingMode.Default, stringFormat: "{0:MM/dd/yyyy}")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(100, 30, 0, 0)),

                    // Description
                    new Label()
                        .Text("Description:")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(0, 60, 0, 0)),
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.Description))
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(100, 60, 0, 0)),

                    // Frequency and Interval
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.Frequency), stringFormat: "Frequency: {0}")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(0, 100, 0, 0)),
                    new Label().Bind(Label.TextProperty, nameof(ViewModel.Interval), stringFormat: "Interval: {0}")
                        .FontSize(14)
                        .Row(Row.Info)
                        .Column(Column.One)
                        .Margin(new Thickness(200, 100, 0, 0)),

                    // Occurrences stub
                    new Label()
                        .Text("Upcoming Occurrences:")
                        .FontSize(18)
                        .Bold()
                        .Row(Row.OccurrencesLabel)
                        .Column(Column.One)
                        .Margin(new Thickness(0, 150, 0, 0)),

                    // Replace with proper CollectionView when implemented
                    new Label()
                        .Text("[Occurrences list will display here]")
                        .FontSize(14)
                        .Italic()
                        .Row(Row.OccurrencesList)
                        .Column(Column.One)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadScheduledJobCommand.Execute(ScheduledJobDefinitionId);
    }

    private enum Row
    {
        Title,
        Info,
        OccurrencesLabel,
        OccurrencesList
    }

    private enum Column
    {
        One
    }
}