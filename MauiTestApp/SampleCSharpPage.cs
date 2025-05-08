namespace MauiTestApp;

public class SampleCSharpPage : ContentPage
{
	public SampleCSharpPage()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Text = "Welcome to .NET MAUI!"
				}
			},
			BackgroundColor = Colors.Green
		};
	}
}