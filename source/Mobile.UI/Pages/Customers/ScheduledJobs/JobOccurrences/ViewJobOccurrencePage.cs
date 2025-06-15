using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;
using Mobile.UI.Styles;
using Server.Contracts.Dtos;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

[CustomerIdQueryParam]
[JobOccurrenceIdQueryParam]
[ScheduledJobDefinitionIdQueryParam]
public class ViewJobOccurrencePage : BasePage<ViewJobOccurrenceModel>
{
    public string JobOccurrenceId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    public ViewJobOccurrencePage(ViewJobOccurrenceModel vm) : base(vm)
    {
        Title = "Job Occurrence Details";
        BackgroundColor = CardStyles.Colors.Background;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    // Job Details Card
                    CreateJobDetailsCard(),

                    // Status Card
                    CreateStatusCard(),

                    // Actions Card
                    CreateActionsCard(),

                    // Photos Section
                    CreatePhotosSection(),

                    // Completed date (prints “null” if not set)
                    new Label()
                        .Bind(Label.TextProperty, nameof(ViewModel.CompletedDate),
                            stringFormat: "Completed: {0:MMM d, yyyy h:mm tt}",
                            fallbackValue: "Completed: null")
                        .FontSize(16)
                        .Bind(IsVisibleProperty, nameof(vm.MarkedAsComplete)),
                    //
                    // // Mark as Completed
                    // new Button()
                    //     .Text("Mark as Completed")
                    //     .Bind(IsVisibleProperty, nameof(ViewModel.CanMarkComplete))
                    //     .Bind(Button.CommandProperty, nameof(ViewModel.MarkCompletedCommand)),
                    //
                    // // Upload Photo
                    // new Button()
                    //     .Text("Upload Photo")
                    //     .IsEnabled(true)
                    //     .Bind(Button.CommandProperty, nameof(ViewModel.UploadPhotoCommand)),
                    //
                    // // Create Invoice
                    // new Button()
                    //     .Text("Create Invoice")
                    //     .Bind(Button.CommandProperty, nameof(ViewModel.CreateInvoiceCommand)),
                    // new Button()
                    //     .Text("Go Back")
                    //     .IsEnabled(true)
                    //     .Bind(Button.CommandProperty, nameof(ViewModel.GoBackCommand)),
                    new CollectionView
                        {
                            ItemTemplate = new(() =>
                            {
                                var layout = new VerticalStackLayout();
                                layout.Children.Add(new Image()
                                    .Height(150)
                                    .Aspect(Aspect.AspectFill)
                                    .Bind(Image.SourceProperty, nameof(PhotoDisplayItem.Path)));
                                layout.Children.Add(new Button()
                                    .Text("Remove")
                                    .Bind(Button.CommandParameterProperty, ".")
                                    .Bind(Button.CommandProperty, nameof(ViewModel.RemovePhotoCommand)));
                                return layout;
                            })
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.PhotoPaths))
                        .IsEnabled(true)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadCommand.Execute(new CustomerJobAndOccurrenceIds(
            new CustomerId(Guid.Parse(CustomerId)),
            new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId)),
            new JobOccurrenceId(Guid.Parse(JobOccurrenceId))));
    }

    private Frame CreateJobDetailsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Job title
                CardStyles.CreateTitleLabel()
                    .Bind(Label.TextProperty, nameof(ViewModel.JobTitle)),

                // Occurrence date with icon
                CardStyles.CreateIconTextStack("📅",
                    CardStyles.CreateSubtitleLabel()
                        .Bind(Label.TextProperty, nameof(ViewModel.OccurrenceDate),
                            stringFormat: "Occurred: {0:MMM d, yyyy h:mm tt}"))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Primary);
    }

    private Frame CreateStatusCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("📊 Completion Status"),

                // Completed date (when completed)
                CardStyles.CreateIconTextStack("✅",
                        CardStyles.CreateSubtitleLabel()
                            .Bind(Label.TextProperty, nameof(ViewModel.CompletedDate),
                                stringFormat: "Completed: {0:MMM d, yyyy h:mm tt}",
                                fallbackValue: "Not completed yet"))
                    .Bind(IsVisibleProperty, nameof(ViewModel.MarkedAsComplete)),

                // Mark as completed button
                CardStyles.CreatePrimaryButton("✅ Mark as Completed")
                    .Bind(IsVisibleProperty, nameof(ViewModel.CanMarkComplete))
                    .Bind(Button.CommandProperty, nameof(ViewModel.MarkCompletedCommand))
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Success);
    }

    private Frame CreateActionsCard()
    {
        var content = new VerticalStackLayout
        {
            Spacing = CardStyles.Spacing.ItemSpacing,
            Children =
            {
                // Section title
                CardStyles.CreateTitleLabel()
                    .Text("🎯 Actions"),

                // Action buttons in a grid
                new Grid
                {
                    ColumnDefinitions = Columns.Define(
                        (Column.Left, Star),
                        (Column.Right, Star)
                    ),
                    RowDefinitions = Rows.Define(
                        (Row.Top, Auto),
                        (Row.Bottom, Auto)
                    ),
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                    Children =
                    {
                        // Upload Photo
                        CardStyles.CreatePrimaryButton("📷 Upload Photo")
                            .Bind(Button.CommandProperty, nameof(ViewModel.UploadPhotoCommand))
                            .Column(Column.Left).Row(Row.Top),

                        // Create Invoice
                        CardStyles.CreateSecondaryButton("📄 Create Invoice", CardStyles.Colors.Warning)
                            .Bind(Button.CommandProperty, nameof(ViewModel.CreateInvoiceCommand))
                            .Column(Column.Right).Row(Row.Top),

                        // Go Back
                        CardStyles.CreateSecondaryButton("⬅️ Go Back")
                            .Bind(Button.CommandProperty, nameof(ViewModel.GoBackCommand))
                            .Column(Column.Left).Row(Row.Bottom)
                            .ColumnSpan(2)
                    }
                }
            }
        };

        return CardStyles.CreateCard(content, CardStyles.Colors.Warning);
    }

    private VerticalStackLayout CreatePhotosSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                // Section header
                CardStyles.CreateTitleLabel()
                    .Text("📸 Job Photos"),

                // Photos collection
                new CollectionView
                    {
                        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
                        {
                            ItemSpacing = 12
                        },
                        SelectionMode = SelectionMode.None,
                        EmptyView = CreateEmptyPhotosView(),
                        ItemTemplate = new DataTemplate(CreatePhotoCard)
                    }
                    .Bind(ItemsView.ItemsSourceProperty, nameof(ViewModel.PhotoPaths))
            }
        };
    }

    private Frame CreatePhotoCard()
    {
        var content = new Grid
        {
            ColumnDefinitions = Columns.Define(
                (Column.Content, Star),
                (Column.Action, Auto)
            ),
            Children =
            {
                // Photo image
                new Image
                    {
                        HeightRequest = 200,
                        Aspect = Aspect.AspectFill,
                        BackgroundColor = CardStyles.Colors.Background
                    }
                    .Bind(Image.SourceProperty, nameof(PhotoDisplayItem.Path))
                    .Column(Column.Content),

                // Remove button
                CardStyles.CreateSecondaryButton("🗑️ Remove", CardStyles.Colors.Error)
                    .Bind(Button.CommandParameterProperty, ".")
                    .Bind(Button.CommandProperty, nameof(ViewModel.RemovePhotoCommand))
                    .Column(Column.Action)
            }
        };

        return CardStyles.CreateCard(content);
    }

    private static VerticalStackLayout CreateEmptyPhotosView() =>
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
                    Text = "📷",
                    FontSize = 48,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "No Photos Yet",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = CardStyles.Colors.TextPrimary,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "Upload photos to document your work.",
                    FontSize = 16,
                    TextColor = CardStyles.Colors.TextSecondary,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            }
        };

    private enum Column
    {
        Left,
        Right,
        Content,
        Action
    }

    private enum Row
    {
        Top,
        Bottom
    }
}