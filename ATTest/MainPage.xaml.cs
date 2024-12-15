using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Feed;
using FishyFlip.Models;
using FishyFlip.Tools;
using MPowerKit.VirtualizeListView;

namespace ATTest;

public partial class MainPage : ContentPage
{
	private string cursor = string.Empty;

	private ATProtocol atProtocol;
	public MainPage()
	{
		InitializeComponent();
		var atProtocolBuilder = new ATProtocolBuilder();
		atProtocol = atProtocolBuilder.Build();
		this.LoadFeedAsync().FireAndForgetSafeAsync();
	}

	public ObservableRangeCollection<FeedViewPost> Feed { get; set; } = new ObservableRangeCollection<FeedViewPost>();

	public async Task LoadFeedAsync()
	{
		var (result, error) = await atProtocol.Feed.GetAuthorFeedAsync(ATIdentifier.Create("drasticactions.dev")!, cursor: cursor);
		if (result is not null)
		{
			cursor = result.Cursor;
			this.Feed.AddRange(result.Feed ?? Array.Empty<FeedViewPost>().ToList());
		}
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
               System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }