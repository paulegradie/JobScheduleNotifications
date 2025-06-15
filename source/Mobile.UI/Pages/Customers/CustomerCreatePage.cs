﻿using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers;

public sealed class CustomerCreatePage : BasePage<CustomerCreateModel>
{
    public CustomerCreatePage(CustomerCreateModel vm) : base(vm)
    {
        Title = "Add Customer";
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Clear all fields when the page appears to ensure fresh form
        ViewModel.ClearFieldsCommand.Execute(null);
    }

    private Frame CreatePersonalInfoCard(CustomerCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("👤 Personal Information"),

                // First name field
                CreateFormField("First Name", "Enter first name...",
                    nameof(vm.FirstName), vm),

                // Last name field
                CreateFormField("Last Name", "Enter last name...",
                    nameof(vm.LastName), vm)
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateContactInfoCard(CustomerCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📞 Contact Information"),

                // Email field
                CreateFormField("Email", "Enter email address...",
                    nameof(vm.Email), vm, Keyboard.Email),

                // Phone field
                CreateFormField("Phone", "Enter phone number...",
                    nameof(vm.PhoneNumber), vm, Keyboard.Telephone)
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Frame CreateAdditionalInfoCard(CustomerCreateModel vm)
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📝 Additional Information"),

                // Notes field
                new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label
                        {
                            Text = "Notes:",
                            FontSize = CardStyles.Typography.CaptionSize,
                            TextColor = CardStyles.Colors.TextSecondary
                        },
                        new Editor
                            {
                                HeightRequest = 120,
                                Placeholder = "Enter any additional notes...",
                                BackgroundColor = Colors.White,
                                TextColor = CardStyles.Colors.TextPrimary
                            }
                            .Bind(Editor.TextProperty, nameof(vm.Notes), source: vm)
                    }
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Warning);
    }

    private Frame CreateActionsCard(CustomerCreateModel vm)
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
                        FontSize = CardStyles.Typography.SubtitleSize,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(vm.ErrorMessage), source: vm)
                    .Bind(IsVisibleProperty, nameof(vm.ErrorMessage), source: vm),

                // Action buttons
                new Grid
                {
                    ColumnDefinitions = Columns.Define(
                        (Column.Save, Star),
                        (Column.Cancel, Star)
                    ),
                    ColumnSpacing = 12,
                    Children =
                    {
                        // Save button
                        CardStyles.CreatePrimaryButton("💾 Save Customer")
                            .BindCommand(nameof(vm.SaveCommand), source: vm)
                            .Column(Column.Save),

                        // Cancel button
                        CardStyles.CreateSecondaryButton("❌ Cancel")
                            .BindCommand(nameof(vm.CancelCommand), source: vm)
                            .Column(Column.Cancel)
                    }
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private VerticalStackLayout CreateFormField(string label, string placeholder,
        string bindingPath, CustomerCreateModel vm, Keyboard? keyboard = null)
    {
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
                new Entry
                    {
                        Placeholder = placeholder,
                        BackgroundColor = Colors.White,
                        TextColor = CardStyles.Colors.TextPrimary,
                        Keyboard = keyboard ?? Keyboard.Default
                    }
                    .Bind(Entry.TextProperty, bindingPath, source: vm)
            }
        };
    }

    private enum Column
    {
        Save,
        Cancel
    }
}