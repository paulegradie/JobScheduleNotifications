using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;


[CustomerIdQueryParam]
[JobOccurrenceIdQueryParam]
[ScheduledJobDefinitionIdQueryParam]
public sealed class InvoiceCreatePage : BasePage<InvoiceCreateModel>
{
    public string CustomerId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }
    public string JobOccurrenceId { get; set; }
    public string JobDescription { get; set; }

    public InvoiceCreatePage(InvoiceCreateModel vm) : base(vm)
    {
        Title = "Create Invoice";
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    // Invoice Header Card
                    CreateInvoiceHeaderCard(),

                    // Add Item Card
                    CreateAddItemCard(),

                    // Invoice Items Card
                    CreateInvoiceItemsCard(),

                    // Total & Actions Card
                    CreateTotalActionsCard(),

                    // PDF Preview Card
                    CreatePdfPreviewCard()
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.Initialize(CustomerId, ScheduledJobDefinitionId, JobOccurrenceId);//, JobDescription);
    }

    private Frame CreateInvoiceHeaderCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📄 Invoice Information"),

                // Invoice date section
                new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        CardStyles.CreateIconTextStack("📅",
                            new Label
                            {
                                Text = "Invoice Date:",
                                FontSize = CardStyles.Typography.CaptionSize,
                                TextColor = CardStyles.Colors.TextSecondary
                            }),
                        CardStyles.CreateSubtitleLabel()
                            .Bind(Label.TextProperty, nameof(ViewModel.Today))
                    }
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateAddItemCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("➕ Add Invoice Item"),

                // Item description entry
                new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label
                        {
                            Text = "Description:",
                            FontSize = CardStyles.Typography.CaptionSize,
                            TextColor = CardStyles.Colors.TextSecondary
                        },
                        new Entry
                        {
                            Placeholder = "Enter item description...",
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(ViewModel.CurrentItemDescription))
                    }
                },

                // Price entry
                new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label
                        {
                            Text = "Price ($):",
                            FontSize = CardStyles.Typography.CaptionSize,
                            TextColor = CardStyles.Colors.TextSecondary
                        },
                        new Entry
                        {
                            Placeholder = "0.00",
                            Keyboard = Keyboard.Numeric,
                            BackgroundColor = Colors.White,
                            TextColor = CardStyles.Colors.TextPrimary
                        }
                        .Bind(Entry.TextProperty, nameof(ViewModel.CurrentItemPrice))
                    }
                },

                // Add button
                CardStyles.CreatePrimaryButton("➕ Add Item")
                    .Bind(Button.CommandProperty, nameof(ViewModel.AddItemCommand))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Frame CreateInvoiceItemsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📋 Invoice Items"),

                // Items collection
                new CollectionView
                {
                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                    {
                        ItemSpacing = 8
                    },
                    SelectionMode = SelectionMode.None,
                    EmptyView = CreateEmptyItemsView(),
                    ItemTemplate = new DataTemplate(() => CreateInvoiceItemCard())
                }
                .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.InvoiceItems))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Warning);
    }

    private Frame CreateInvoiceItemCard()
    {
        var content = new Grid
        {
            ColumnDefinitions = Columns.Define(
                (Column.Description, Star),
                (Column.Price, Auto)
            ),
            Children =
            {
                // Item description
                CardStyles.CreateSubtitleLabel()
                    .Bind(Label.TextProperty, "Description")
                    .Column(Column.Description),

                // Item price
                new Label
                {
                    FontSize = CardStyles.Typography.SubtitleSize,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardStyles.Colors.Success,
                    HorizontalOptions = LayoutOptions.End
                }
                .Bind(Label.TextProperty, "Price", stringFormat: "${0:F2}")
                .Column(Column.Price)
            }
        };

        return new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = CardStyles.Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Margin = new Thickness(0, 2),
            Content = content
        };
    }

    private Frame CreateTotalActionsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("💰 Total & Actions"),

                // Total amount
                CardStyles.CreateIconTextStack("💵",
                    new Label
                    {
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = CardStyles.Colors.Success
                    }
                    .Bind(Label.TextProperty, nameof(ViewModel.Total), stringFormat: "Total: ${0:F2}")),

                // Generate PDF button
                CardStyles.CreatePrimaryButton("📄 Generate PDF")
                    .Bind(Button.CommandProperty, nameof(ViewModel.GeneratePdfCommand))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Frame CreatePdfPreviewCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("👁️ PDF Preview"),

                // Preview container
                new Grid
                {
                    HeightRequest = 500,
                    Children =
                    {
                        // No preview message
                        new VerticalStackLayout
                        {
                            Spacing = 16,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    Text = "📄",
                                    FontSize = 48,
                                    HorizontalOptions = LayoutOptions.Center
                                },
                                new Label
                                {
                                    Text = "No Preview Yet",
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold,
                                    TextColor = CardStyles.Colors.TextPrimary,
                                    HorizontalOptions = LayoutOptions.Center
                                },
                                new Label
                                {
                                    Text = "Generate a PDF to see the preview here.",
                                    FontSize = 14,
                                    TextColor = CardStyles.Colors.TextSecondary,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center
                                }
                            }
                        }
                        .Bind(IsVisibleProperty, nameof(ViewModel.PreviewFilePath),
                            convert: (string? path) => string.IsNullOrWhiteSpace(path)),

                        // PDF WebView
                        new WebView
                        {
                            BackgroundColor = Colors.White
                        }
                        .Bind(IsVisibleProperty, nameof(ViewModel.PreviewFilePath),
                            convert: (string? path) => !string.IsNullOrWhiteSpace(path))
                        .Bind(WebView.SourceProperty, nameof(ViewModel.PreviewFilePath), convert: (string? path) =>
                        {
#if ANDROID
                            return $"file://{path}";
#elif WINDOWS
                            return path;
#elif IOS
                            return new Foundation.NSUrl(path, false).AbsoluteString;
#else
                            return path;
#endif
                        })
                    }
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private static VerticalStackLayout CreateEmptyItemsView() =>
        new VerticalStackLayout
        {
            Spacing = 16,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(40),
            Children =
            {
                new Label
                {
                    Text = "📋",
                    FontSize = 48,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "No Items Added",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardStyles.Colors.TextPrimary,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Add items to create your invoice.",
                    FontSize = 16,
                    TextColor = CardStyles.Colors.TextSecondary,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

    private enum Column
    {
        Description,
        Price
    }
}