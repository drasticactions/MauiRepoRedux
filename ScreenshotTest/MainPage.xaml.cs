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

		var result = await this.Window!.CaptureAsync();
		var fileStream = await result.OpenReadAsync();
		var image = new Image { Source = ImageSource.FromStream(() => fileStream) };
		var bytes = await result.OpenReadAsync();
		var buffer = new byte[bytes.Length];
		await bytes.ReadAsync(buffer.AsMemory(0, buffer.Length));
		var localDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		var filePath = Path.Combine(localDocs, "screenshot.png");
		File.WriteAllBytes(filePath, buffer);
		DotNetBotImage.Source = image.Source;
	}
}

