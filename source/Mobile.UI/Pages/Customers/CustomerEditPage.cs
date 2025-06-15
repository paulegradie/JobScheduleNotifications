using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;

namespace Mobile.UI.Pages.Customers;

[CustomerIdQueryParam]
public sealed class CustomerEditPage : BasePage<CustomerEditModel>
{
    public string CustomerId { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadCustomerCommand.ExecuteAsync(CustomerId);
    }

    public CustomerEditPage(CustomerEditModel vm) : base(vm)
    {
        Title = vm.Title;
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    // Personal Information Card
                    CreatePersonalInfoCard(vm),

                    // Contact Information Card
                    CreateContactInfoCard(vm),

                    // Additional Information Card
                    CreateAdditionalInfoCard(vm),

                    // Actions Card
                    CreateActionsCard(vm)
                }
            }
        };
    }

    private Frame CreatePersonalInfoCard(CustomerEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel().Text("👤 Personal Information"),

                // First Name entry
                Section("First Name",
                    new Entry
                        {
                            Placeholder = "Enter first name...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.FirstName))
                ),

                // Last Name entry
                Section("Last Name",
                    new Entry
                        {
                            Placeholder = "Enter last name...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.LastName))
                )
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateContactInfoCard(CustomerEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📞 Contact Information"),

                // Email entry
                Section("Email",
                    new Entry
                        {
                            Placeholder = "Enter email address...",
                            Keyboard = Keyboard.Email,
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.Email))
                ),

                // Phone entry
                Section("Phone Number",
                    new Entry
                        {
                            Placeholder = "Enter phone number...",
                            Keyboard = Keyboard.Telephone,
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(vm.PhoneNumber))
                )
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Frame CreateAdditionalInfoCard(CustomerEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📝 Additional Information"),

                // Notes editor
                Section("Notes",
                    new Editor
                        {
                            HeightRequest = 100,
                            Placeholder = "Enter additional notes...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Editor.TextProperty, nameof(vm.Notes))
                )
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Warning);
    }

    private Frame CreateActionsCard(CustomerEditModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Error message
                new Label
                    {
                        TextColor = CardStyles.Colors.Error,
                        FontSize = CardStyles.Typography.SubtitleSize
                    }
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                    .Bind(IsVisibleProperty, nameof(vm.ErrorMessage)),

                // Button container
                new HorizontalStackLayout
                {
                    Spacing = 12,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        // Save button
                        CardStyles.CreatePrimaryButton("💾 Save Customer")
                            .BindCommand(nameof(vm.SaveCustomerCommand)),
                            // .Bind(IsEnabledProperty, nameof(vm.CanSave)),

                        // Cancel button
                        CardStyles.CreateSecondaryButton("❌ Cancel", CardStyles.Colors.TextSecondary)
                            .BindCommand(nameof(vm.CancelCommand))
                    }
                },

                // Loading indicator
                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                    .Bind(IsVisibleProperty, nameof(vm.IsBusy))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private static VerticalStackLayout Section(string label, View control) =>
        new()
        {
            Spacing = 4,
            Children =
            {
                new Label().Text(label).Font(size: 14, bold: true),
                control
            }
        };
}