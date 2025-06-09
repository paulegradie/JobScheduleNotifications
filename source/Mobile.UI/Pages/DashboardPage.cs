using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class DashboardPage : BasePage<DashboardViewModel>
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = vm.Title;

        /* Direct colour constants (no resource lookup) */
        var blue = Colors.Blue; // primary action
        var green = Colors.Green; // success
        var red = Colors.Red; // danger
        var grayText = Colors.Gray;

        BackgroundColor = Colors.White;

        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = 20,
                RowDefinitions = Rows.Define(
                    (Row.Header, Auto),
                    (Row.Stats, Auto),
                    (Row.Actions, Auto),
                    (Row.Loader, Auto)),
                Children =
                {
                    BuildHeader(vm, grayText).Row(Row.Header),
                    BuildStatsGrid(vm).Row(Row.Stats),
                    BuildActionButtons(blue, green, red).Row(Row.Actions),
                    BuildBusyOverlay(vm, blue).Row(Row.Loader)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        // TODO: if we ever support multi-org, then we'll have an org selection page, that will set param on the route and we'll take that here and pass it to the execute command
        base.OnAppearing();
        ViewModel.LoadDashboardDataCommand.Execute(null);
    }


    /* ---------- header block ---------- */
    static VerticalStackLayout BuildHeader(DashboardViewModel vm, Color grayText) =>
        new()
        {
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.BusinessName))
                    .Font(size: 24, bold: true)
                    .TextColor(grayText),
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.WelcomeMessage))
                    .Font(size: 24, bold: true)
                    .TextColor(grayText),


                new Label()
                    .Text("Here's an overview of your business")
                    .FontSize(16)
                    .TextColor(grayText)
            }
        };

    /* ---------- 2×2 stats grid ---------- */
    [Obsolete("Obsolete")]
    static Grid BuildStatsGrid(DashboardViewModel vm)
    {
        /* local palette */
        var custText = Color.FromArgb("#1976D2");
        var custBg = Color.FromArgb("#E3F2FD");
        var jobsText = Color.FromArgb("#388E3C");
        var jobsBg = Color.FromArgb("#E8F5E9");
        var pendText = Color.FromArgb("#F57C00");
        var pendBg = Color.FromArgb("#FFF3E0");
        var compText = Color.FromArgb("#7B1FA2");
        var compBg = Color.FromArgb("#F3E5F5");

        return new Grid
        {
            ColumnDefinitions = Columns.Define((Col.Left, Star), (Col.Right, Star)),
            RowDefinitions = Rows.Define((StatRow.Top, Auto), (StatRow.Middle, Auto), (StatRow.Bottom, Auto)),
            ColumnSpacing = 10,
            RowSpacing = 10,
            Margin = new Thickness(0, 20, 0, 20),

            Children =
            {
                StatFrame("Total Customers", nameof(vm.TotalCustomers), custText, custBg)
                    .Column(Col.Left).Row(StatRow.Top),
                StatFrame("Total Jobs", nameof(vm.TotalJobs), jobsText, jobsBg)
                    .Column(Col.Right).Row(StatRow.Top),
                StatFrame("In Progress Jobs", nameof(vm.InProgressJobs), jobsText, jobsBg)
                    .Column(Col.Right).Row(StatRow.Middle),
                StatFrame("UnInvoiced Jobs", nameof(vm.UnInvoicedJobs), pendText, pendBg)
                    .Column(Col.Left).Row(StatRow.Middle),
                StatFrame("Total Jobs", nameof(vm.TotalJobs), jobsText, jobsBg)
                    .Column(Col.Right).Row(StatRow.Bottom),
                StatFrame("Completed Jobs", nameof(vm.CompletedJobs), compText, compBg)
                    .Column(Col.Right).Row(StatRow.Bottom)
            }
        };

        static Frame StatFrame(string label, string vmProp, Color text, Color bg) =>
            new Frame
            {
                CornerRadius = 10,
                Padding = 15,
                BackgroundColor = bg,
                HasShadow = false,
                Content = new VerticalStackLayout
                {
                    Children =
                    {
                        new Label().Text(label).FontSize(14).TextColor(text),
                        new Label().Bind(Label.TextProperty, vmProp)
                            .Font(size: 24, bold: true).TextColor(text)
                    }
                }
            };
    }

    /* ---------- action buttons ---------- */
    VerticalStackLayout BuildActionButtons(Color blue, Color green, Color red) =>
        new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                SolidButton("Manage Customers", nameof(_vm.NavigateToCustomersCommand), blue),
                SolidButton("Logout", nameof(_vm.LogoutCommand), red)
            }
        };

    static Button SolidButton(string text, string commandName, Color bg) =>
        new Button()
            .Text(text)
            .BindCommand(commandName)
            .BackgroundColor(bg)
            .TextColor(Colors.White)
            .FontSize(16)
            .Height(50);

    /* ---------- busy overlay ---------- */
    static ActivityIndicator BuildBusyOverlay(DashboardViewModel vm, Color accent) =>
        new ActivityIndicator()
            .Center()
            .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
            .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy));

    /* enum helpers */
    enum Row
    {
        Header,
        Stats,
        Actions,
        Loader
    }

    enum Col
    {
        Left,
        Right
    }

    enum StatRow
    {
        Top,
        Middle,
        Bottom
    }
}