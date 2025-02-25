using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using Microsoft.Maui.Adapters;

namespace TestUI;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		this.listView.Adapter = this.Collection;
	}

	public TestCollection Collection { get; } = new TestCollection();
}

public class TestCollection : VirtualListViewAdapterBase<object, FeedViewPost>
{
	private GetAuthorFeedOutputCollection items;

	public TestCollection()
	{
		var atProtocolBuilder = new ATProtocolBuilder();
		var atProtocol = atProtocolBuilder.Build();
		this.items = atProtocol.Feed.GetAuthorFeedCollectionAsync(ATIdentifier.Create("drasticactions.jp")!);
		this.items.CollectionChanged += (s, e) => this.InvalidateData();
		this.items.GetMoreItemsAsync().FireAndForgetSafeAsync();
	}

    public override FeedViewPost GetItem(int sectionIndex, int itemIndex)
    {
		return this.items[itemIndex];
    }

    public override int GetNumberOfItemsInSection(int sectionIndex)
    {
		return this.items.Count;
    }
}


    /// <summary>
    /// Task Utilities.
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Fire and Forget Safe Async.
        /// </summary>
        /// <param name="task">Task to Fire and Forget.</param>
        /// <param name="handler">Error Handler.</param>
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
            }
        }
    }