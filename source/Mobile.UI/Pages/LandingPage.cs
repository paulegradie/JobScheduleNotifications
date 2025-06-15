using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class LandingPage : BasePage<LandingPageModel>
{
    private readonly LandingPageModel _vm;

    public LandingPage(LandingPageModel vm) : base(vm)
    {
        _vm = vm;
        Title = "ServicePro";

        /*──────── visual tree ────────*/
        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = new Thickness(20),
                BackgroundColor = CardStyles.Colors.Background,
                RowDefinitions = Rows.Define(
                    (Row.Header, Auto),
                    (Row.Features, Auto),
                    (Row.Actions, Auto),
                    (Row.Footer, Auto)),

                Children =
                {
                    /*─── header block ───*/
                    BuildHeader().Row(Row.Header),

                    /*─── features showcase ───*/
                    BuildFeaturesSection().Row(Row.Features),

                    /*─── action buttons ───*/
                    BuildActionButtons().Row(Row.Actions),

                    /*─── footer ───*/
                    BuildFooter().Row(Row.Footer)
                }
            }
        };
    }

    private VerticalStackLayout BuildHeader() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            Margin = new Thickness(0, 40, 0, 32),
            Children =
            {
                // Brand logo area (using service icon)
                new Frame
                {
                    BackgroundColor = CardStyles.Colors.Primary,
                    CornerRadius = 30,
                    HasShadow = false,
                    Padding = new Thickness(20),
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = "🔧",
                        FontSize = 40,
                        HorizontalOptions = LayoutOptions.Center
                    }
                },

                // Brand name and tagline
                new Label()
                    .Text("ServicePro")
                    .Font(size: 36, bold: true)
                    .TextColor(CardStyles.Colors.Primary)
                    .CenterHorizontal(),

                new Label()
                    .Text("Professional Service Management")
                    .Font(size: 18, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary)
                    .CenterHorizontal(),

                new Label()
                    .Text("Streamline your service business with smart scheduling, customer management, and automated workflows")
                    .FontSize(16)
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .CenterHorizontal()
                    .Margin(new Thickness(20, 8, 20, 0))
            }
        };

    private VerticalStackLayout BuildFeaturesSection() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            Margin = new Thickness(0, 0, 0, 32),
            Children =
            {
                new Label()
                    .Text("Why Choose ServicePro?")
                    .Font(size: 24, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary)
                    .CenterHorizontal()
                    .Margin(new Thickness(0, 0, 0, 16)),

                CreateFeatureCard("📋", "Smart Scheduling", "Automated job scheduling with recurring appointments and intelligent reminders"),
                CreateFeatureCard("👥", "Customer Management", "Complete customer profiles with contact info, job history, and notes"),
                CreateFeatureCard("📸", "Job Documentation", "Photo capture and documentation for completed work"),
                CreateFeatureCard("💰", "Invoicing & Billing", "Automated invoice generation and email delivery")
            }
        };

    private Frame CreateFeatureCard(string icon, string title, string description) =>
        CardStyles.CreateCard(
            new HorizontalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = icon,
                        FontSize = 24,
                        VerticalOptions = LayoutOptions.Start
                    },
                    new VerticalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            CardStyles.CreateTitleLabel()
                                .Text(title)
                                .FontSize(16),
                            CardStyles.CreateSubtitleLabel()
                                .Text(description)
                                .FontSize(14)
                        }
                    }
                }
            });

    private VerticalStackLayout BuildActionButtons() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            Margin = new Thickness(0, 0, 0, 32),
            Children =
            {
                CreateActionButton("🚀 Get Started", "Sign In", nameof(_vm.NavigateToLoginCommand), CardStyles.Colors.Primary),
                CreateActionButton("📝 Create Account", "New to ServicePro?", nameof(_vm.NavigateToRegisterCommand), CardStyles.Colors.Success),
                CreateActionButton("👀 Quick Demo", "View Sample Data", nameof(_vm.NavigateToCustomersCommand), CardStyles.Colors.Warning)
            }
        };

    private Frame CreateActionButton(string title, string subtitle, string commandName, Color backgroundColor) =>
        CardStyles.CreateCard(
            new Button
            {
                BackgroundColor = backgroundColor,
                TextColor = Colors.White,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 12,
                Padding = new Thickness(20, 16),
                Text = title,
                HorizontalOptions = LayoutOptions.FillAndExpand
            }
            .BindCommand(commandName));

    private VerticalStackLayout BuildFooter() =>
        new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label()
                    .Text("©2025 ServicePro")
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .FontSize(14)
                    .CenterHorizontal(),
                new Label()
                    .Text("Professional Service Management Platform")
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .FontSize(12)
                    .CenterHorizontal()
            }
        };

    /* enum helpers for Grid placement */
    private enum Row
    {
        Header,
        Features,
        Actions,
        Footer
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if DEBUG
        await _vm.AutoLoginForDev();
#endif

        try
        {
            if (SemanticScreenReader.Default != null)
            {
                SemanticScreenReader.Announce("Welcome to ServicePro");
            }
        }
        catch (Exception)
        {
            System.Diagnostics.Debug.WriteLine("SemanticScreenReader is not available");
        }
    }
}