namespace Mobile.UI.Styles;

/// <summary>
/// Shared styling system for consistent card designs across the application
/// </summary>
public static class CardStyles
{
    // Color palette
    public static class Colors
    {
        public static readonly Color Primary = Color.FromArgb("#2196F3");
        public static readonly Color TextPrimary = Color.FromArgb("#000000");
        public static readonly Color TextSecondary = Color.FromArgb("#000000");
        public static readonly Color CardBorder = Color.FromArgb("#E1E5E9");
        public static readonly Color Background = Color.FromArgb("#F8F9FA");
        public static readonly Color Success = Color.FromArgb("#4CAF50");
        public static readonly Color Warning = Color.FromArgb("#FF9800");
        public static readonly Color Error = Color.FromArgb("#F44336");
    }

    // Typography
    public static class Typography
    {
        public const int TitleSize = 18;
        public const int SubtitleSize = 14;
        public const int CaptionSize = 13;
        public const int IconSize = 14;
    }

    // Spacing
    public static class Spacing
    {
        public static readonly Thickness CardPadding = new(20, 16);
        public static readonly Thickness CardMargin = new(0, 6);
        public static readonly Thickness ButtonPadding = new(16, 10);
        public const double CardSpacing = 12;
        public const double ItemSpacing = 8;
        public const double IconSpacing = 6;
    }

    /// <summary>
    /// Creates a standard card frame with consistent styling
    /// </summary>
    public static Frame CreateCard(View content, Color? accentColor = null)
    {
        var card = new Frame
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            BorderColor = Colors.CardBorder,
            CornerRadius = 12,
            HasShadow = false,
            Padding = 0,
            Margin = Spacing.CardMargin
        };

        var container = new VerticalStackLayout
        {
            Spacing = 0
        };

        // Add accent bar if specified
        if (accentColor != null)
        {
            container.Children.Add(new BoxView
            {
                BackgroundColor = accentColor,
                HeightRequest = 4
            });
        }

        // Add content with padding
        var contentContainer = new VerticalStackLayout
        {
            Padding = Spacing.CardPadding,
            Children = { content }
        };

        container.Children.Add(contentContainer);
        card.Content = container;

        return card;
    }

    /// <summary>
    /// Creates a primary action button with consistent styling
    /// </summary>
    public static Button CreatePrimaryButton(string text)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = Colors.Primary,
            TextColor = Microsoft.Maui.Graphics.Colors.White,
            CornerRadius = 8,
            FontSize = Typography.SubtitleSize,
            Padding = Spacing.ButtonPadding,
            VerticalOptions = LayoutOptions.Center
        };
    }

    /// <summary>
    /// Creates a secondary action button with consistent styling
    /// </summary>
    public static Button CreateSecondaryButton(string text, Color? backgroundColor = null)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = backgroundColor ?? Colors.TextSecondary,
            TextColor = Microsoft.Maui.Graphics.Colors.White,
            CornerRadius = 6,
            FontSize = Typography.CaptionSize,
            Padding = new Thickness(12, 8),
            VerticalOptions = LayoutOptions.Center
        };
    }

    /// <summary>
    /// Creates a title label with consistent styling
    /// </summary>
    public static Label CreateTitleLabel()
    {
        return new Label
        {
            FontSize = Typography.TitleSize,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.TextPrimary
        };
    }

    /// <summary>
    /// Creates a subtitle label with consistent styling
    /// </summary>
    public static Label CreateSubtitleLabel()
    {
        return new Label
        {
            FontSize = Typography.SubtitleSize,
            TextColor = Colors.TextSecondary
        };
    }

    /// <summary>
    /// Creates a caption label with consistent styling
    /// </summary>
    public static Label CreateCaptionLabel()
    {
        return new Label
        {
            FontSize = Typography.CaptionSize,
            TextColor = Colors.TextSecondary
        };
    }

    /// <summary>
    /// Creates an icon label with consistent styling
    /// </summary>
    public static Label CreateIconLabel(string icon)
    {
        return new Label
        {
            Text = icon,
            FontSize = Typography.IconSize
        };
    }

    /// <summary>
    /// Creates a horizontal stack with icon and text
    /// </summary>
    public static HorizontalStackLayout CreateIconTextStack(string icon, Label textLabel)
    {
        return new HorizontalStackLayout
        {
            Spacing = Spacing.IconSpacing,
            Children =
            {
                CreateIconLabel(icon),
                textLabel
            }
        };
    }

    /// <summary>
    /// Creates a styled Entry with consistent border and visual feedback
    /// </summary>
    public static Frame CreateStyledEntry(string placeholder = "", string bindingPath = "")
    {
        var entry = new Entry
        {
            Placeholder = placeholder,
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            TextColor = Colors.TextPrimary,
            FontSize = Typography.SubtitleSize
        };

        if (!string.IsNullOrEmpty(bindingPath))
        {
            entry.SetBinding(Entry.TextProperty, bindingPath);
        }

        return new Frame
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            BorderColor = Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = entry
        };
    }

    /// <summary>
    /// Creates a styled Editor with consistent border and visual feedback
    /// </summary>
    public static Frame CreateStyledEditor(string placeholder = "", string bindingPath = "", double heightRequest = 100)
    {
        var editor = new Editor
        {
            Placeholder = placeholder,
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            TextColor = Colors.TextPrimary,
            FontSize = Typography.SubtitleSize,
            HeightRequest = heightRequest
        };

        if (!string.IsNullOrEmpty(bindingPath))
        {
            editor.SetBinding(Editor.TextProperty, bindingPath);
        }

        return new Frame
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            BorderColor = Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = editor
        };
    }

    /// <summary>
    /// Creates a styled DatePicker with consistent border and visual feedback
    /// </summary>
    public static Frame CreateStyledDatePicker(string bindingPath = "")
    {
        var datePicker = new DatePicker
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            TextColor = Colors.TextPrimary,
            FontSize = Typography.SubtitleSize
        };

        if (!string.IsNullOrEmpty(bindingPath))
        {
            datePicker.SetBinding(DatePicker.DateProperty, bindingPath);
        }

        return new Frame
        {
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
            BorderColor = Colors.CardBorder,
            CornerRadius = 8,
            HasShadow = false,
            Padding = new Thickness(12, 8),
            Content = datePicker
        };
    }
}
