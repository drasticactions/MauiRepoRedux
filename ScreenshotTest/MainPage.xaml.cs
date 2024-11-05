namespace ScreenshotTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

		var result = await Screenshot.CaptureAsync();
		var fileStream = await result.OpenReadAsync();
		var image = new Image { Source = ImageSource.FromStream(() => fileStream) };
		DotNetBotImage.Source = image.Source;
	}
}

