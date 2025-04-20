namespace Mobile.Pages;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private int count = 0;
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		
		// Access the ViewModel and set up SemanticScreenReader announcements
		if (BindingContext is Mobile.PageModels.MainPageViewModel viewModel)
		{
			viewModel.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(Mobile.PageModels.MainPageViewModel.CounterText))
				{
					SemanticScreenReader.Announce(viewModel.CounterText);
				}
			};
		}
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;
		Console.WriteLine($"Clicked {count} times!");
	}
}

