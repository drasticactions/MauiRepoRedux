namespace MauiRepoRedux;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		try
		{
			var options = new PickOptions() { FileTypes = FilePickerFileType.Pdf, PickerTitle = "Please select a PDF file" };
			var fileResultList = await FilePicker.PickMultipleAsync(options);
			if (fileResultList != null)
			{
				//Other logic
				PdfLabel.Text = $"You selected {fileResultList.Count()} files";
			}
		}
		catch (Exception ex)
		{
			//Other logic
		}
	}
}

