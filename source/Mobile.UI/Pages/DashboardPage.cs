﻿﻿﻿﻿﻿﻿﻿using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class DashboardPage : BasePage<DashboardViewModel>
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = vm.Title;

        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = new Thickness(16),
                RowDefinitions = Rows.Define(
                    (Row.Header, Auto),
                    (Row.Stats, Auto),
                    (Row.Actions, Auto),
                    (Row.Loader, Auto)),
                Children =
                {
                    BuildHeader(vm).Row(Row.Header),
                    BuildStatsGrid(vm).Row(Row.Stats),
                    BuildActionButtons().Row(Row.Actions),
                    BuildBusyOverlay(vm).Row(Row.Loader)
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
    static VerticalStackLayout BuildHeader(DashboardViewModel vm) =>
        new()
        {
            Spacing = 12,
            Margin = new Thickness(0, 0, 0, 24),
            Children =
            {
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.BusinessName))
                    .Font(size: 28, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary),
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.WelcomeMessage))
                    .Font(size: 20, bold: true)
                    .TextColor(CardStyles.Colors.Primary),
                new Label()
                    .Text("Here's an overview of your business")
                    .FontSize(16)
                    .TextColor(CardStyles.Colors.TextSecondary)
            }
        };

    /* ---------- beautiful stats grid ---------- */
    static Grid BuildStatsGrid(DashboardViewModel vm)
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define((Col.Left, Star), (Col.Right, Star)),
            RowDefinitions = Rows.Define((StatRow.Top, Auto), (StatRow.Bottom, Auto)),
            ColumnSpacing = 12,
            RowSpacing = 12,
            Margin = new Thickness(0, 0, 0, 24),

            Children =
            {
                CreateStatCard("👥", "Total Customers", nameof(vm.TotalCustomers), CardStyles.Colors.Primary)
                    .Column(Col.Left).Row(StatRow.Top),
                CreateStatCard("📋", "Total Jobs", nameof(vm.TotalJobs), CardStyles.Colors.Success)
                    .Column(Col.Right).Row(StatRow.Top),
                CreateStatCard("⏳", "In Progress", nameof(vm.InProgressJobs), CardStyles.Colors.Warning)
                    .Column(Col.Left).Row(StatRow.Bottom),
                CreateStatCard("✅", "Completed", nameof(vm.CompletedJobs), CardStyles.Colors.Success)
                    .Column(Col.Right).Row(StatRow.Bottom)
            }
        };
    }

    static Frame CreateStatCard(string icon, string label, string vmProperty, Color accentColor)
    {
        var content = new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                // Icon and label row
                new HorizontalStackLayout
                {
                    Spacing = 8,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = icon,
                            FontSize = 20
                        },
                        CardStyles.CreateSubtitleLabel()
                            .Text(label)
                    }
                },

                // Large number
                new Label()
                    .Bind(Label.TextProperty, vmProperty)
                    .Font(size: 32, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary)
            }
        };

        return CardStyles.CreateCard(content, accentColor);
    }

    /* ---------- action buttons ---------- */
    VerticalStackLayout BuildActionButtons() =>
        new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                CreateActionButton("👥 Manage Customers", nameof(_vm.NavigateToCustomersCommand), CardStyles.Colors.Primary),
                CreateActionButton("🚪 Logout", nameof(_vm.LogoutCommand), CardStyles.Colors.Error)
            }
        };

    Button CreateActionButton(string text, string commandName, Color backgroundColor) =>
        new Button
        {
            Text = text,
            BackgroundColor = backgroundColor,
            TextColor = Colors.White,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 12,
            Padding = new Thickness(20, 16),
            HorizontalOptions = LayoutOptions.FillAndExpand
        }
        .BindCommand(commandName);

    /* ---------- busy overlay ---------- */
    static ActivityIndicator BuildBusyOverlay(DashboardViewModel vm) =>
        new ActivityIndicator
        {
            Color = CardStyles.Colors.Primary,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }
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
        Bottom
    }
}