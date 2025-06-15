using System.Globalization;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;

namespace Mobile.UI.Pages;

public sealed class RegisterPage : BasePage<RegisterViewModel>
{
    public RegisterPage(RegisterViewModel vm) : base(vm)
    {
        Shell.SetNavBarIsVisible(this, false);
        Title = "Create Account - ServicePro";

        Content = new ScrollView
        {
            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.Header, GridRowsColumns.Auto),
                    (Row.Form, GridRowsColumns.Star),
                    (Row.Footer, GridRowsColumns.Auto)
                ),
                Padding = new Thickness(20),
                BackgroundColor = CardStyles.Colors.Background,
                Children =
                {
                    // Header with Branding
                    BuildHeader().Row(Row.Header),

                    // Form
                    BuildRegistrationForm(vm).Row(Row.Form),

                    // Footer
                    BuildFooter(vm).Row(Row.Footer)
                }
            }
        };
    }

    private VerticalStackLayout BuildHeader() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            Margin = new Thickness(0, 20, 0, 24),
            Children =
            {
                // Brand logo
                new Frame
                {
                    BackgroundColor = CardStyles.Colors.Success,
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
                    .Text("Create Your Account")
                    .Font(size: 24, bold: true)
                    .TextColor(CardStyles.Colors.TextPrimary)
                    .CenterHorizontal(),

                new Label()
                    .Text("Join thousands of service professionals")
                    .FontSize(16)
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .CenterHorizontal()
            }
        };

    private VerticalStackLayout BuildRegistrationForm(RegisterViewModel vm) =>
        new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                // Business Information Section
                CreateSectionHeader("🏢 Business Information"),
                CreateFieldDescription("This information will appear on your invoices and customer communications"),
                CreateStyledEntryWithPlaceholder("Business Name", "e.g. Smith's Cleaning Services", nameof(vm.BusinessName)),

                // Business Address Section
                CreateSubSectionHeader("📍 Business Address"),
                CreateStyledEntryWithPlaceholder("Street Address", "e.g. 123 Main Street, Unit 5", nameof(vm.BusinessAddress)),
                CreateAddressRow(vm),
                CreateStyledEntryWithPlaceholder("Country", "e.g. Australia", nameof(vm.BusinessCountry)),

                // Business Identification
                CreateSubSectionHeader("🆔 Business Identification"),
                CreateStyledEntryWithPlaceholder("ABN / Business ID (Optional)", "e.g. 12 345 678 901", nameof(vm.BusinessId)),

                // Bank Details Section
                CreateSectionHeader("💳 Payment Information"),
                CreateFieldDescription("Bank details for customer payments (will appear on invoices)"),
                CreateStyledEntryWithPlaceholder("Bank Details", "e.g. BSB: 123-456, Account: 12345678, Commonwealth Bank", nameof(vm.BankDetails)),

                // Personal Information Section
                CreateSectionHeader("👤 Personal Information"),
                CreateFieldDescription("Your contact information as the business owner/manager"),
                CreateStyledEntryWithPlaceholder("First Name", "e.g. John", nameof(vm.FirstName)),
                CreateStyledEntryWithPlaceholder("Last Name", "e.g. Smith", nameof(vm.LastName)),
                CreateStyledEntryWithPlaceholder("Phone Number", "e.g. 0412 345 678", nameof(vm.PhoneNumber)),

                // Account Information Section
                CreateSectionHeader("🔐 Account Information"),
                CreateStyledEntryWithPlaceholder("Email", "e.g. john@smithscleaning.com.au", nameof(vm.Email)),
                CreatePasswordEntry(vm, "Password", nameof(vm.Password), nameof(vm.IsPasswordVisible), nameof(vm.TogglePasswordVisibilityCommand)),
                CreatePasswordEntry(vm, "Confirm Password", nameof(vm.ConfirmPassword), nameof(vm.IsConfirmPasswordVisible), nameof(vm.ToggleConfirmPasswordVisibilityCommand)),

                // Error message
                new Label()
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                    .TextColor(CardStyles.Colors.Error)
                    .Bind(Label.IsVisibleProperty, nameof(vm.ErrorMessage),
                        converter: new StringToBoolConverter())
                    .FontSize(14)
                    .CenterHorizontal(),

                // Create Account button
                CardStyles.CreatePrimaryButton("🚀 Create Account")
                    .BindCommand(nameof(vm.RegisterCommand)),

                // Activity indicator
                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                    .CenterHorizontal()
            }
        };

    private Label CreateSectionHeader(string text) =>
        new Label()
            .Text(text)
            .Font(size: 18, bold: true)
            .TextColor(CardStyles.Colors.TextPrimary)
            .Margin(new Thickness(0, 16, 0, 8));

    private Label CreateSubSectionHeader(string text) =>
        new Label()
            .Text(text)
            .Font(size: 16, bold: true)
            .TextColor(CardStyles.Colors.TextSecondary)
            .Margin(new Thickness(0, 12, 0, 4));

    private Label CreateFieldDescription(string text) =>
        new Label()
            .Text(text)
            .FontSize(14)
            .TextColor(CardStyles.Colors.TextSecondary)
            .Margin(new Thickness(0, 0, 0, 8));

    private Frame CreateStyledEntryWithPlaceholder(string label, string placeholder, string bindingPath) =>
        new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = CardStyles.Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = new Entry
            {
                Placeholder = placeholder,
                BackgroundColor = Colors.Transparent,
                TextColor = CardStyles.Colors.TextPrimary,
                PlaceholderColor = CardStyles.Colors.TextSecondary
            }
            .Bind(Entry.TextProperty, bindingPath)
        };

    private HorizontalStackLayout CreateAddressRow(RegisterViewModel vm) =>
        new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Frame
                {
                    BackgroundColor = Colors.White,
                    BorderColor = CardStyles.Colors.CardBorder,
                    CornerRadius = 8,
                    HasShadow = false,
                    Padding = new Thickness(12, 8),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Content = new Entry
                    {
                        Placeholder = "e.g. Melbourne",
                        BackgroundColor = Colors.Transparent,
                        TextColor = CardStyles.Colors.TextPrimary,
                        PlaceholderColor = CardStyles.Colors.TextSecondary
                    }
                    .Bind(Entry.TextProperty, nameof(vm.BusinessCity))
                },
                new Frame
                {
                    BackgroundColor = Colors.White,
                    BorderColor = CardStyles.Colors.CardBorder,
                    CornerRadius = 8,
                    HasShadow = false,
                    Padding = new Thickness(12, 8),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Content = new Entry
                    {
                        Placeholder = "e.g. Victoria",
                        BackgroundColor = Colors.Transparent,
                        TextColor = CardStyles.Colors.TextPrimary,
                        PlaceholderColor = CardStyles.Colors.TextSecondary
                    }
                    .Bind(Entry.TextProperty, nameof(vm.BusinessState))
                }
            }
        };

    private Frame CreatePasswordEntry(RegisterViewModel vm, string placeholder, string textBinding, string visibilityBinding, string toggleCommand) =>
        new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = CardStyles.Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = new Grid
            {
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (Column.First, GridRowsColumns.Star),
                    (Column.Second, GridRowsColumns.Auto)),
                Children =
                {
                    new Entry()
                        .Placeholder(placeholder)
                        .Bind(Entry.TextProperty, textBinding)
                        .Bind(Entry.IsPasswordProperty, visibilityBinding,
                            converter: new InvertedBoolConverter())
                        .BackgroundColor(Colors.Transparent)
                        .TextColor(CardStyles.Colors.TextPrimary)
                        .Column(0),
                    new Button()
                        .Text("👁")
                        .BindCommand(toggleCommand)
                        .BackgroundColor(Colors.Transparent)
                        .TextColor(CardStyles.Colors.TextSecondary)
                        .Padding(8, 0)
                        .Column(1)
                }
            }
        };

    private HorizontalStackLayout BuildFooter(RegisterViewModel vm) =>
        new HorizontalStackLayout
        {
            Spacing = 5,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 16, 0, 20),
            Children =
            {
                new Label()
                    .Text("Already have an account?")
                    .TextColor(CardStyles.Colors.TextSecondary)
                    .FontSize(14),

                new Label
                {
                    Text = "Sign In",
                    TextColor = CardStyles.Colors.Primary,
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextDecorations = TextDecorations.Underline,
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer()
                            .Bind(TapGestureRecognizer.CommandProperty, nameof(vm.LoginCommand))
                    }
                }
            }
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