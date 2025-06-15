﻿﻿﻿﻿﻿﻿using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class LoginPage : BasePage<LoginViewModel>
{
    public LoginPage(LoginViewModel vm) : base(vm)
    {
        Shell.SetNavBarIsVisible(this, false);
        Title = "Sign In - ServicePro";

        Content = new ScrollView
        {
            Content = new Grid
            {
                BackgroundColor = CardStyles.Colors.Background,
                Padding = new Thickness(20),
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.Header, Auto),
                    (Row.Content, Star),
                    (Row.Footer, Auto)
                ),

                Children =
                {
                    // ─── Header with Branding ───────────────────────────────────────────
                    BuildHeader().Row(Row.Header),

                    // ─── Login Form ───────────────────────────────────────
                    BuildLoginForm(vm).Row(Row.Content),

                    // ─── Footer: Register Link ─────────────────────────────
                    BuildFooter(vm).Row(Row.Footer)
                }
            }
        };
    }

    private VerticalStackLayout BuildHeader() =>
        new()
        {
            Spacing = 16,
            Margin = new Thickness(0, 40, 0, 32),
            Children =
            {
                // Brand logo
                new Frame
                {
                    BackgroundColor = CardStyles.Colors.Primary,
                    CornerRadius = 25,
                    HasShadow = false,
                    Padding = new Thickness(16),
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = "🔧",
                        FontSize = 32,
                        HorizontalOptions = LayoutOptions.Center
                    }
                },

                new Label()
                    .Text("ServicePro")
                    .Font(size: 28, bold: true)
                    .TextColor(CardStyles.Colors.Primary)
                    .CenterHorizontal(),

                new Label()
                    .Text("Welcome Back!")
                    .Font(size: 24, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary)
                    .CenterHorizontal(),

                new Label()
                    .Text("Sign in to manage your service business")
                    .FontSize(16)
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .CenterHorizontal()
            }
        };

    private VerticalStackLayout BuildLoginForm(LoginViewModel vm) =>
        new VerticalStackLayout
        {
            Spacing = 20,
            Children =
            {
                // Email input
                CardStyles.CreateStyledEntry("Email", nameof(vm.Email)),

                // Password input with toggle
                CreatePasswordEntry(vm),

                // Error message
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                    .Bind(Label.IsVisibleProperty, nameof(vm.ErrorMessage))
                    .TextColor(CardStyles.Colors.Error)
                    .FontSize(14)
                    .CenterHorizontal(),

                // Sign In button
                CardStyles.CreatePrimaryButton("🚀 Sign In")
                    .BindCommand(nameof(vm.LoginCommand)),

                // Go Back button
                CardStyles.CreateSecondaryButton("← Go Back")
                    .BindCommand(nameof(vm.NavigateBackCommand)),

                // Activity indicator
                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                    .CenterHorizontal()
            }
        }
        .CenterVertical();

    private Frame CreatePasswordEntry(LoginViewModel vm) =>
        new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = CardStyles.Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Star, Auto),
                Children =
                {
                    new Entry()
                        .Placeholder("Password")
                        .Bind(Entry.TextProperty, nameof(vm.Password))
                        .Bind(Entry.IsPasswordProperty, nameof(vm.IsPasswordVisible),
                            converter: new InvertedBoolConverter())
                        .BackgroundColor(Colors.Transparent)
                        .Column(0),
                    new Button()
                        .Text("👁")
                        .BindCommand(nameof(vm.TogglePasswordVisibilityCommand))
                        .BackgroundColor(Colors.Transparent)
                        .TextColor(CardStyles.Colors.TextSecondary)
                        .Padding(8, 0)
                        .Column(1)
                }
            }
        };

    private HorizontalStackLayout BuildFooter(LoginViewModel vm) =>
        new HorizontalStackLayout
        {
            Spacing = 5,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                new Label
                {
                    Text = "Don't have an account?",
                    TextColor = CardStyles.Colors.TextSecondary,
                    FontSize = 14
                },
                new Label
                {
                    Text = "Sign Up",
                    TextColor = CardStyles.Colors.Primary,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextDecorations = TextDecorations.Underline,
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = vm.NavigateToRegisterCommand
                        }
                    }
                }
            }
        };

    enum Row
    {
        Header,
        Content,
        Footer
    }
}