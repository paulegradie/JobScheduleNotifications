using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class DashboardPage : BasePage<DashboardViewModel>
{
    public DashboardPage(DashboardViewModel vm) : base(vm)
    {
        Title = vm.Title;

        /* Direct colour constants (no resource lookup) */
        var blue = Colors.Blue; // primary action  :contentReference[oaicite:1]{index=1}
        var green = Colors.Green; // success         :contentReference[oaicite:2]{index=2}
        var red = Colors.Red; // danger          :contentReference[oaicite:3]{index=3}
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
                    BuildActionButtons(vm, blue, green, red).Row(Row.Actions),
                    BuildBusyOverlay(vm, blue).Row(Row.Loader)
                }
            }
        };
    }

    /* ---------- header block ---------- */
    static VerticalStackLayout BuildHeader(DashboardViewModel vm, Color grayText) =>
        new VerticalStackLayout
        {
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                new Label()
                    .Bind(Label.TextProperty, nameof(Core.PageModels.DashboardViewModel.WelcomeMessage))
                    .Font(size: 24, bold: true),

                new Label()
                    .Text("Here's an overview of your business")
                    .FontSize(16)
                    .TextColor(grayText)
            }
        };

    /* ---------- 2×2 stats grid ---------- */
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
            RowDefinitions = Rows.Define((StatRow.Top, Auto), (StatRow.Bottom, Auto)),
            ColumnSpacing = 10,
            RowSpacing = 10,
            Margin = new Thickness(0, 20, 0, 20),

            Children =
            {
                StatFrame("Total Customers", nameof(Core.PageModels.DashboardViewModel.TotalCustomers), custText, custBg)
                    .Column(Col.Left).Row(StatRow.Top),

                StatFrame("Total Jobs", nameof(Core.PageModels.DashboardViewModel.TotalJobs), jobsText, jobsBg)
                    .Column(Col.Right).Row(StatRow.Top),

                StatFrame("Pending Jobs", nameof(Core.PageModels.DashboardViewModel.PendingJobs), pendText, pendBg)
                    .Column(Col.Left).Row(StatRow.Bottom),

                StatFrame("Completed Jobs", nameof(Core.PageModels.DashboardViewModel.CompletedJobs), compText, compBg)
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
    static VerticalStackLayout BuildActionButtons(DashboardViewModel vm, Color blue, Color green, Color red) =>
        new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                SolidButton("Manage Customers", nameof(Core.PageModels.DashboardViewModel.NavigateToCustomersCommand), blue),
                SolidButton("Schedule New Job", nameof(Core.PageModels.DashboardViewModel.NavigateToScheduleJobCommand), green),
                SolidButton("Logout", nameof(Core.PageModels.DashboardViewModel.LogoutCommand), red)
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
            .Bind(ActivityIndicator.IsRunningProperty, nameof(Core.PageModels.DashboardViewModel.IsBusy))
            .Bind(ActivityIndicator.IsVisibleProperty, nameof(Core.PageModels.DashboardViewModel.IsBusy));

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
        Bottom
    }
}