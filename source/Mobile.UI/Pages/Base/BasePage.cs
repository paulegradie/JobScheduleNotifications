using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;

namespace Mobile.UI.Pages.Base;

/// Non‑generic base for every page in the app.
/// •Adds consistent padding  
/// •Logs lifecycle events (handy while debugging)  
/// •Optionally auto‑registers Toolkit messenger recipients
public abstract class BasePage : ContentPage // ContentPage is MAUI’s root page type:contentReference[oaicite:0]{index=0}
{
    protected BasePage(object? viewModel = null)
    {
        Padding = 12;
        BindingContext = viewModel;

        RegisterMessengerRecipients(viewModel);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Trace.WriteLine($"[Page] Appearing{Title}");
    }

    protected override void OnDisappearing()
    {
        Trace.WriteLine($"[Page] Disappearing{Title}");
        base.OnDisappearing();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        RegisterMessengerRecipients(BindingContext);
    }

    private void RegisterMessengerRecipients(object? vm)
    {
        if (vm is null) return;

        var implementsRecipient = vm.GetType()
            .GetInterfaces()
            .Any(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(IRecipient<>));

        if (implementsRecipient)
            WeakReferenceMessenger.Default.RegisterAll(vm);
    }
}

/// Strongly‑typed wrapper so derived pages can access their
/// view‑model without casting.
public abstract partial class BasePage<TViewModel>(TViewModel vm)
    : BasePage(vm)
    where TViewModel : class
{
    protected new TViewModel BindingContext
    {
        get { return (TViewModel)base.BindingContext; }
        set { base.BindingContext = value; }
    }
}