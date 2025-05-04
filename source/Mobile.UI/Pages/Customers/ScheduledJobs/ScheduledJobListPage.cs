using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

[QueryProperty(nameof(CustomerId), "customerId")]
public sealed class ScheduledJobListPage : BasePage<ScheduledJobListModel>
{
    public string CustomerId { get; set; }

    public ScheduledJobListPage(ScheduledJobListModel vm) : base(vm)
    {
        Title = "List All Scheduled Jobs for this Customer:";
        Content = new RefreshView()
            {
                Content = new CollectionView()
                    {
                        SelectionMode = SelectionMode.None,
                        EmptyView = "No scheduled jobs found",
                        ItemTemplate = new DataTemplate(() => new Frame
                        {
                            Padding = 10,
                            CornerRadius = 10,
                            Content = BuildGrid(ViewModel),
                            BackgroundColor = Colors.LightGray,
                            HasShadow = true,
                            Margin = new Thickness(10, 10, 10, 10),
                            HeightRequest = 200,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                        })
                    }
                    .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.ScheduledJobs))
            }
            .Bind(RefreshView.IsRefreshingProperty, nameof(vm.IsBusy));
    }

    private Grid BuildGrid(ScheduledJobListModel model) =>
        new Grid
        {
            Margin = new Thickness(2, 10, 2, 0),
            RowDefinitions = Rows.Define((Row.Title, Auto), (Row.CustomerId, Auto), (Row.AnchorDate, Auto), (Row.EditButton, Auto)),
            Children =
            {
                new Label()
                    .Font(size: 16, bold: true)
                    .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.Title))
                    .Row(Row.Title).Column(0),

                new Label()
                    .Font(size: 12)
                    .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.CustomerId))
                    .Row(Row.CustomerId).Column(0),

                new Label()
                    .Font(size: 12)
                    .Bind(Label.TextProperty, nameof(ScheduledJobDefinitionDto.AnchorDate), stringFormat: "{0:yyyy-MM-dd}")
                    .Row(Row.AnchorDate).Column(0),

                new Button()
                    .Text("Edit")
                    .Bind(Button.CommandProperty, nameof(model.NavigateToEditCommand), source: model)
                    .Bind(Button.CommandParameterProperty, ".")
                    .Row(Row.EditButton)
                    .RowSpan(3).Column(1)
                    .CenterVertical()
            }
        };


    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadForCustomerCommand.Execute(CustomerId);
    }

    private enum Row
    {
        Title,
        CustomerId,
        AnchorDate,
        EditButton,
    }
}