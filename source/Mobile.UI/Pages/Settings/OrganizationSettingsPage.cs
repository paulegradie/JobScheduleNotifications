﻿﻿using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Settings;

public class OrganizationSettingsPage : BasePage<OrganizationSettingsViewModel>
{
    public OrganizationSettingsPage(OrganizationSettingsViewModel viewModel) : base(viewModel)
    {
        Title = "Organization Settings";
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = CardStyles.Spacing.CardSpacing,
                Padding = new Thickness(16),
                Children =
                {
                    CreatePageHeader(),
                    CreateBusinessInfoCard(),
                    CreateContactInfoCard(),
                    CreateAddressCard(),
                    CreateBankingCard(),
                    CreateActionsCard()
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }

    private Frame CreatePageHeader()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreateTitleLabel()
                    .Text("⚙️ Organization Settings"),
                new Label
                {
                    Text = "Manage your business information, contact details, and banking information for invoices.",
                    FontSize = CardStyles.Typography.CaptionSize,
                    TextColor = CardStyles.Colors.TextSecondary
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateBusinessInfoCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreateTitleLabel()
                    .Text("🏢 Business Information"),

                new Label { Text = "Business Name", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("Enter your business name", nameof(ViewModel.BusinessName)),

                new Label { Text = "Business Description", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("Describe your business", nameof(ViewModel.BusinessDescription)),

                new Label { Text = "Business ID (ABN/Tax Number)", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("e.g. 12 345 678 901", nameof(ViewModel.BusinessIdNumber))
            }
        };

        return CardStyles.CreateCard(content);
    }

    private Frame CreateContactInfoCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreateTitleLabel()
                    .Text("📞 Contact Information"),

                new Label { Text = "Email Address", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("business@example.com", nameof(ViewModel.Email)),

                new Label { Text = "Phone Number", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("+61 400 000 000", nameof(ViewModel.PhoneNumber))
            }
        };

        return CardStyles.CreateCard(content);
    }

    private Frame CreateAddressCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreateTitleLabel()
                    .Text("📍 Business Address"),

                new Label { Text = "Street Address", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("123 Main Street", nameof(ViewModel.StreetAddress)),

                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Star),
                    ColumnSpacing = 12,
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "City", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("Melbourne", nameof(ViewModel.City))
                            }
                        }.Column(0),

                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "State", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("VIC", nameof(ViewModel.State))
                            }
                        }.Column(1)
                    }
                },

                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Star),
                    ColumnSpacing = 12,
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "Postal Code", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("3000", nameof(ViewModel.PostalCode))
                            }
                        }.Column(0),

                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "Country", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("Australia", nameof(ViewModel.Country))
                            }
                        }.Column(1)
                    }
                }
            }
        };

        return CardStyles.CreateCard(content);
    }

    private Frame CreateBankingCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreateTitleLabel()
                    .Text("💳 Banking Information"),

                new Label 
                { 
                    Text = "This information will appear on invoices for customer payments.",
                    FontSize = CardStyles.Typography.CaptionSize,
                    TextColor = CardStyles.Colors.TextSecondary
                },

                new Label { Text = "Bank Name", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("Commonwealth Bank", nameof(ViewModel.BankName)),

                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Star),
                    ColumnSpacing = 12,
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "BSB", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("123-456", nameof(ViewModel.BankBsb))
                            }
                        }.Column(0),

                        new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "Account Number", FontSize = CardStyles.Typography.CaptionSize },
                                CardStyles.CreateStyledEntry("12345678", nameof(ViewModel.BankAccountNumber))
                            }
                        }.Column(1)
                    }
                },

                new Label { Text = "Account Name", FontSize = CardStyles.Typography.CaptionSize },
                CardStyles.CreateStyledEntry("Your Business Name", nameof(ViewModel.BankAccountName))
            }
        };

        return CardStyles.CreateCard(content);
    }

    private Frame CreateActionsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                CardStyles.CreatePrimaryButton("💾 Save Settings")
                    .BindCommand(nameof(ViewModel.SaveCommand))
                    .Bind(Button.IsEnabledProperty, nameof(ViewModel.CanSave)),

                new ActivityIndicator
                    {
                        Color = CardStyles.Colors.Primary
                    }
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy))
                    .Bind(IsVisibleProperty, nameof(ViewModel.IsBusy))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }
}
