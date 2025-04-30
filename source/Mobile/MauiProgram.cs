using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Mobile.Resources.Fonts;
using Microsoft.Extensions.Logging;
using Mobile.Composition;
using Syncfusion.Maui.Toolkit.Hosting;

// for AppWindow APIs

// for SizeInt32
namespace Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

// #if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.AddConsole();

// #endif
        builder.Services.ComposeServices();

        var app = builder.Build();
        //  ─── catch WinUI (Windows) XAML exceptions ─────────────────────────────────────────────────
#if WINDOWS
        Microsoft.UI.Xaml.Application.Current.UnhandledException += (winSender, winArgs) =>
        {
            var log = app.Services.GetRequiredService<ILogger<ErrorWatcher>>();
            log.LogError(winArgs.Exception, "WinUI UnhandledException");
            // winArgs.Handled = true; // if you want to swallow it
        };
#endif

        //  ─── catch any AppDomain‐level exceptions (cross‐platform) ────────────────────────────────────
        AppDomain.CurrentDomain.UnhandledException += (domSender, domArgs) =>
        {
            var ex = domArgs.ExceptionObject as Exception;
            var log = app.Services.GetRequiredService<ILogger<ErrorWatcher>>();
            log.LogError(ex, "AppDomain UnhandledException");
        };

        //  ─── catch unobserved Task‐based exceptions ─────────────────────────────────────────────────
        TaskScheduler.UnobservedTaskException += (tskSender, tskArgs) =>
        {
            var log = app.Services.GetRequiredService<ILogger<ErrorWatcher>>();
            log.LogError(tskArgs.Exception, "UnobservedTaskException");
            tskArgs.SetObserved();
        };
        return app;
    }
}

public class ErrorWatcher;