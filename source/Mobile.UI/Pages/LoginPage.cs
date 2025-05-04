using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;

using Mobile.UI.Pages.Base;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class LoginPage : BasePage<LoginViewModel>
{
    public LoginPage(LoginViewModel vm) : base(vm)
    {
        Shell.SetNavBarIsVisible(this, false);

        // Theme‑aware brushes
        var bgColor = Application.Current.RequestedTheme == AppTheme.Light
            ? Colors.White
            : Color.FromArgb("#1C1C1E");
        var textPrimary = Application.Current.RequestedTheme == AppTheme.Light
            ? Color.FromArgb("#1C1C1E")
            : Colors.White;
        var textSecondary = Application.Current.RequestedTheme == AppTheme.Light
            ? Color.FromArgb("#666666")
            : Color.FromArgb("#8E8E93");
        var frameBorder = Application.Current.RequestedTheme == AppTheme.Light
            ? Color.FromArgb("#E5E5EA")
            : Color.FromArgb("#2C2C2E");
        var frameBg = Application.Current.RequestedTheme == AppTheme.Light
            ? Color.FromArgb("#F2F2F7")
            : Color.FromArgb("#2C2C2E");
        var buttonBg = Application.Current.RequestedTheme == AppTheme.Light
            ? Color.FromArgb("#007AFF")
            : Color.FromArgb("#0A84FF");

        Content = new Grid
        {
            BackgroundColor = bgColor,
            Padding = 20,
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.Header, Auto),
                (Row.Content, Star),
                (Row.Footer, Auto)
            ),

            Children =
            {
                // ─── Header ───────────────────────────────────────────
                new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Label()
                                .Text("Welcome Back!")
                                .Font(size: 32, bold: true)
                                .TextColor(textPrimary)
                                .CenterHorizontal(),

                            new Label()
                                .Text("Sign in to continue")
                                .FontSize(16)
                                .TextColor(textSecondary)
                                .CenterHorizontal()
                        }
                    }
                    .Margin(new Thickness(0, 40, 0, 20))
                    .Row(Row.Header),


                // ─── Login Form ───────────────────────────────────────
                new VerticalStackLayout
                    {
                        Spacing = 20,
                        Children =
                        {
                            // Email
                            new Frame
                            {
                                BorderColor = frameBorder,
                                BackgroundColor = frameBg,
                                CornerRadius = 10,
                                HasShadow = false,
                                Padding = new Thickness(15, 0),

                                Content = new Entry()
                                    .Placeholder("Email")
                                    .Bind(Entry.TextProperty, nameof(vm.Email))
                            },

                            // Password + eye toggle
                            new Frame
                            {
                                BorderColor = frameBorder,
                                BackgroundColor = frameBg,
                                CornerRadius = 10,
                                HasShadow = false,
                                Padding = new Thickness(15, 0),

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
                                            .Margin(10, 0)
                                            .Column(2),
                                        new Button()
                                            .Bind(Button.TextProperty, nameof(vm.IsPasswordVisible),
                                                converter: new BoolToObjectConverter())
                                            .BindCommand(nameof(vm.TogglePasswordVisibilityCommand))
                                            .BackgroundColor(Colors.Transparent)
                                            .Padding(10, 0)
                                            .Column(1)
                                    }
                                }
                            },

                            // Error message (only visible when non‑empty)
                            new Label()
                                .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                                .Bind(Label.IsVisibleProperty, nameof(vm.ErrorMessage))
                                .TextColor(Colors.Red)
                                .CenterHorizontal(),

                            // Sign In button
                            new Button()
                                .Text("Sign In")
                                .BindCommand(nameof(vm.LoginCommand))
                                .Bind(IsEnabledProperty, nameof(vm.IsBusy))
                                .TextColor(Colors.White)
                                .FontSize(16)
                                .Height(50),
                            // Sign In button
                            new Button()
                                .Text("Go Back")
                                .BindCommand((LoginViewModel vm) => vm.NavigateBackCommand)
                                .Bind(IsEnabledProperty, nameof(vm.IsBusy))
                                .TextColor(Colors.White)
                                .FontSize(16)
                                .Height(50),

                            // Activity indicator
                            new ActivityIndicator
                                {
                                    Color = buttonBg
                                }
                                .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                                .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                                .CenterHorizontal()
                        }
                    }
                    .CenterVertical()
                    .Row(Row.Content),


                // ─── Footer: Register Link ─────────────────────────────
                new HorizontalStackLayout
                    {
                        Spacing = 5,
                        Children =
                        {
                            new Label
                            {
                                Text = "Don't have an account?",
                                TextColor = textSecondary
                            },
                            new Label
                            {
                                Text = "Sign Up",
                                TextColor = buttonBg,
                                TextDecorations = TextDecorations.Underline,
                                // <-- correctly initialize the GestureRecognizers collection
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command = vm.NavigateToRegisterCommand
                                    }
                                }
                            }
                        }
                    }
                    .CenterHorizontal()
                    .Margin(new Thickness(0, 0, 0, 20))
                    .Row(Row.Footer)
            }
        };
    }

    enum Row
    {
        Header,
        Content,
        Footer
    }
}