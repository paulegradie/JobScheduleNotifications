using System;
using System.Globalization;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages;

public sealed class RegisterPage : BasePage<RegisterViewModel>
{
    public RegisterPage(RegisterViewModel vm) : base(vm)
    {
        Shell.SetNavBarIsVisible(this, false);

        Title = "Create Account";

        Content = new ScrollView
        {
            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.Header, GridRowsColumns.Auto),
                    (Row.Form, GridRowsColumns.Star),
                    (Row.Footer, GridRowsColumns.Auto)
                ),
                Padding = 20,
                BackgroundColor = Colors.CadetBlue,
                Children =
                {
                    // Header
                    new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label()
                                    .Text("Create Account")
                                    .Font(size: 32, bold: true)
                                    .TextColor(Colors.CadetBlue)
                                    .CenterHorizontal(),

                                new Label()
                                    .Text("Sign up to get started")
                                    .FontSize(16)
                                    .TextColor(Colors.CadetBlue)
                                    .CenterHorizontal()
                            }
                        }
                        .Margin(new Thickness(0, 40, 0, 20))
                        .Row(Row.Header),

                    // Form
                    new VerticalStackLayout
                        {
                            Spacing = 15,

                            Children =
                            {
                                InputFrame(() => new Entry()
                                    .Placeholder("Email")
                                    .Bind(Entry.TextProperty, nameof(vm.Email))),
                                InputFrame(() => new Grid
                                {
                                    ColumnDefinitions = GridRowsColumns.Columns.Define((Column.First, GridRowsColumns.Star),
                                        (Column.Second, GridRowsColumns.Auto)),

                                    Children =
                                    {
                                        new Entry()
                                            .Placeholder("Password")
                                            .Bind(Entry.TextProperty, nameof(vm.Password))
                                            .Bind(Entry.IsPasswordProperty, nameof(vm.IsPasswordVisible),
                                                converter: new InvertedBoolConverter()),

                                        new Button()
                                            .Bind(Button.TextProperty, nameof(vm.IsPasswordVisible),
                                                converter: new BoolToObjectConverter())
                                            .BindCommand(nameof(vm.TogglePasswordVisibilityCommand))
                                            .BackgroundColor(Colors.Transparent)
                                            .TextColor(Colors.Red)
                                            .FontSize(16)
                                            .Padding(10, 0)
                                            .Column(1)
                                    }
                                }),
                                InputFrame(() => new Grid
                                {
                                    ColumnDefinitions = GridRowsColumns.Columns.Define(
                                        (Column.First, GridRowsColumns.Star),
                                        (Column.Second, GridRowsColumns.Auto)),
                                    Children =
                                    {
                                        new Entry()
                                            .Placeholder("Confirm Password")
                                            .Bind(Entry.TextProperty, nameof(vm.ConfirmPassword))
                                            .Bind(Entry.IsPasswordProperty, nameof(vm.IsConfirmPasswordVisible),
                                                converter: new InvertedBoolConverter()),
                                        new Button()
                                            .Bind(Button.TextProperty, nameof(vm.IsConfirmPasswordVisible),
                                                converter: new BoolToObjectConverter())
                                            .BindCommand(nameof(vm.ToggleConfirmPasswordVisibilityCommand))
                                            .BackgroundColor(Colors.Transparent)
                                            .TextColor(Colors.Black)
                                            .FontSize(16)
                                            .Padding(10, 0)
                                            .Column(1)
                                    }
                                }),
                                InputFrame(() =>
                                    new Entry().Placeholder("Business Name")
                                        .Bind(Entry.TextProperty, nameof(vm.BusinessName))),
                                InputFrame(() =>
                                    new Entry().Placeholder("First Name")
                                        .Bind(Entry.TextProperty, nameof(vm.FirstName))),
                                InputFrame(() =>
                                    new Entry().Placeholder("Last Name")
                                        .Bind(Entry.TextProperty, nameof(vm.LastName))),
                                InputFrame(() =>
                                    new Entry().Placeholder("Phone Number")
                                        .Bind(Entry.TextProperty, nameof(vm.PhoneNumber))),
                                InputFrame(() =>
                                    new Entry().Placeholder("Business Address").Bind(Entry.TextProperty,
                                        nameof(vm.BusinessAddress))),
                                new Label()
                                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                                    .TextColor(Colors.Red)
                                    .Bind(Label.IsVisibleProperty, nameof(vm.ErrorMessage),
                                        converter: new StringToBoolConverter())
                                    .CenterHorizontal(),
                                new Button()
                                    .Text("Create Account")
                                    .BindCommand(nameof(vm.RegisterCommand))
                                    .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy))
                                    .BackgroundColor(Colors.CadetBlue)
                                    .TextColor(Colors.White)
                                    .FontSize(16)
                                    .Height(50),
                                new ActivityIndicator()
                                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                            }
                        }
                        .Row(Row.Form),

                    // Footer
                    new HorizontalStackLayout
                        {
                            Spacing = 5,
                            HorizontalOptions = LayoutOptions.Center,

                            Children =
                            {
                                new Label()
                                    .Text("Already have an account?")
                                    .TextColor(Colors.Red),

                                new Label()
                                    .Text("Sign In")
                                    .TextColor(Colors.White)
                            }
                        }
                        .Margin(new Thickness(0, 0, 0, 20))
                        .Row(Row.Footer)
                }
            }
        };
    }

    private static Frame InputFrame(Func<View> inputFactory) =>
        new Frame
        {
            BorderColor = Colors.Black,
            BackgroundColor = Colors.CadetBlue,
            Padding = new Thickness(15, 0),
            HasShadow = false,
            CornerRadius = 10,
            Content = inputFactory()
        };

    private enum Row
    {
        Header,
        Form,
        Footer
    }

    private enum Column
    {
        First,
        Second
    }
}

public sealed class StringToBoolConverter : IValueConverter
{
    // Converts a non-null, non-empty string to true
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        !string.IsNullOrWhiteSpace(value as string);

    // Not used for one-way bindings
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}