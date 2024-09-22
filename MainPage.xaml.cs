using System.Collections.ObjectModel;

namespace MauiRepoRedux;

public partial class MainPage : ContentPage
{
	int count = 0;
	private Random random;
	
	public MainPage()
	{
		InitializeComponent();
		random = new Random();
		var list = new List<TestItem>();
		for (int i = 0; i < 100; i++)
		{
			list.Add(Generate());
		}
		Items = new ObservableCollection<TestItem>(list);
	}
	
	public string Text { get; set; } = "Hello, Maui!";
	
	public ObservableCollection<TestItem> Items { get; }

	private TestItem Generate()
	{
		return new TestItem
		{
			Name = $"Item {count++}",
			Color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
			Height = random.Next(50, 100)
		};
	}
}

public class TestItem
{
	public string Name { get; set; }
	public Color Color { get; set; }
	
	public int Height { get; set; } = 50;
}

