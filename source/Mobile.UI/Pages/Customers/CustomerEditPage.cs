﻿﻿﻿﻿﻿﻿﻿using CommunityToolkit.Maui.Markup;
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

                // First Name entry with validation
                CreateValidatedField("First Name", "Enter first name...",
                    nameof(vm.FirstName), nameof(vm.IsFirstNameValid), nameof(vm.FirstNameError), vm),

                // Last Name entry with validation
                CreateValidatedField("Last Name", "Enter last name...",
                    nameof(vm.LastName), nameof(vm.IsLastNameValid), nameof(vm.LastNameError), vm)
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

                // Email entry with validation
                CreateValidatedField("Email", "Enter email address...",
                    nameof(vm.Email), nameof(vm.IsEmailValid), nameof(vm.EmailError), vm, Keyboard.Email),

                // Phone entry with validation
                CreateValidatedField("Phone Number", "Enter phone number...",
                    nameof(vm.PhoneNumber), nameof(vm.IsPhoneNumberValid), nameof(vm.PhoneNumberError), vm, Keyboard.Telephone)
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
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.HasError), source: vm),

                // Validation message
                new Label
                    {
                        TextColor = CardStyles.Colors.Warning,
                        FontSize = CardStyles.Typography.CaptionSize,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(vm.ValidationMessage), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.ValidationMessage), source: vm,
                        converter: new StringToBoolConverter()),

                // Button container
                new HorizontalStackLayout
                {
                    Spacing = 12,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children =
                    {
                        // Save button
                        CardStyles.CreatePrimaryButton("💾 Save Customer")
                            .BindCommand(nameof(vm.SaveCustomerCommand), source: vm)
                            .Bind(IsEnabledProperty, nameof(vm.CanSave), source: vm),

                        // Cancel button
                        CardStyles.CreateSecondaryButton("❌ Cancel", CardStyles.Colors.TextSecondary)
                            .BindCommand(nameof(vm.CancelCommand), source: vm)
                    }
                },

                // Loading indicator
                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.IsBusy), source: vm)
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private VerticalStackLayout CreateValidatedField(string label, string placeholder,
        string textBindingPath, string validationBindingPath, string errorBindingPath,
        CustomerEditModel vm, Keyboard? keyboard = null)
    {
        var entry = new Entry
        {
            Placeholder = placeholder,
            TextColor = CardStyles.Colors.TextPrimary,
            Keyboard = keyboard ?? Keyboard.Default
        }
        .Bind(Entry.TextProperty, textBindingPath, source: vm);

        // Bind background color based on validation state
        entry.SetBinding(Entry.BackgroundColorProperty, new Binding(validationBindingPath,
            source: vm,
            converter: new ValidationToColorConverter()));

        var errorLabel = new Label
        {
            FontSize = CardStyles.Typography.CaptionSize,
            TextColor = CardStyles.Colors.Error
        }
        .Bind(Label.TextProperty, errorBindingPath, source: vm)
        .Bind(IsVisibleProperty, errorBindingPath, source: vm,
            converter: new StringToBoolConverter());

        return new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label
                {
                    Text = $"{label}:",
                    FontSize = CardStyles.Typography.CaptionSize,
                    TextColor = CardStyles.Colors.TextSecondary
                },
                entry,
                errorLabel
            }
        };
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