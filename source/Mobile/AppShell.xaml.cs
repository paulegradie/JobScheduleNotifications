﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Mobile.UI.Pages;
using Font = Microsoft.Maui.Font;

namespace Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        RegisterPage<HomePage>();
        RegisterPage<LoginPage>();
        RegisterPage<RegisterPage>();
        RegisterPage<DashboardPage>();
        RegisterPage<CustomersPage>();
        RegisterPage<CreateCustomerPage>();
        RegisterPage<CustomerPage>();
        RegisterPage<CustomerEditPage>();
        RegisterPage<ScheduledJobPage>();
        RegisterPage<AddScheduledJobPage>();

        var currentTheme = Application.Current!.UserAppTheme;
        ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
    }

    private void RegisterPage<TPage>()
    {
        Routing.RegisterRoute(nameof(TPage), typeof(TPage));
    }

    public static async Task DisplaySnackbarAsync(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FF3300"),
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = new CornerRadius(0),
            Font = Font.SystemFontOfSize(18),
            ActionButtonFont = Font.SystemFontOfSize(14)
        };

        var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }

    public static async Task DisplayToastAsync(string message)
    {
        // Toast is currently not working in MCT on Windows
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(message, textSize: 18);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await toast.Show(cts.Token);
    }

    private void SfSegmentedControl_SelectionChanged(object sender,
        Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
    {
        Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
    }
}