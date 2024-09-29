using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiShared;

public partial class TestListPage : ContentPage
{
    public TestListPage()
    {
        InitializeComponent();
    }
    
    public ListViewModel ViewModel { get; } = new ListViewModel();
}

public class ListViewModel
{
    public ListViewModel()
    {
        Items = new ObservableCollection<ListViewItem>(GenerateItems(1000));
    }
	
    public ObservableCollection<ListViewItem> Items { get; }
	
    private List<ListViewItem> GenerateItems(int count)
    {
        var itemsList = new List<ListViewItem>();
        var random = new Random();
        for (int i = 0; i < count; i++)
        {	
            var r = random.Next(0, 255);
            var g = random.Next(0, 255);
            var b = random.Next(0, 255);
            var height = random.Next(50, 150);
            itemsList.Add(new ListViewItem($"Item {i}", Microsoft.Maui.Graphics.Color.FromRgb(r, g, b), height));
        }
        return itemsList;
    }
}

public class ListViewItem(string title, Microsoft.Maui.Graphics.Color color, int height)
{
    public string Title => title;
	
    public Color BackgroundColor => color;
	
    public int Height => height;
}