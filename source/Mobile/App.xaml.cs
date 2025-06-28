namespace Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Initial window dimensions
        const int initialHeight = 715;
        const int initialWidth = 480;

        // Minimum window dimensions (prevents window from becoming too small)
        const int minHeight = 400;
        const int minWidth = 320;

        // Maximum window dimensions (allows for larger sizes)
        // Use double.PositiveInfinity for unlimited size, or set specific max values
        const double maxHeight = double.PositiveInfinity;
        const double maxWidth = double.PositiveInfinity;

        var window = new Window(new AppShell())
        {
            // Set initial size
            Height = initialHeight,
            Width = initialWidth,

            // Set minimum size constraints
            MinimumHeight = minHeight,
            MinimumWidth = minWidth,

            // Set maximum size constraints (allows resizing)
            MaximumHeight = maxHeight,
            MaximumWidth = maxWidth,

            // Optional: Set initial position (centered on screen)
            // You can remove these lines if you want Windows to decide the position
            X = 100,
            Y = 100
        };

        return window;
    }
}