
using Microsoft.Maui.Controls.Embedding;


#if IOS || MACCATALYST
using UIKit;
using CoreGraphics;
using CoreAnimation;
#elif ANDROID
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Java.Nio;
#endif
namespace MauiTestApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object? sender, EventArgs e)
	{
		var page = new SampleCSharpPage();
#if IOS || MACCATALYST
		// Based on https://github.com/dotnet/maui/blob/a835905e81d2b8142f40e32e0e2b1601add6eb25/src/Essentials/src/Screenshot/Screenshot.ios.cs#L57
		// The bug is due to view.Window.Screen.Scale being null, since there is no window.
		var view = page.ToPlatformEmbedded(this.Handler!.MauiContext!);
		view.Frame = new CoreGraphics.CGRect(0, 0, 800, 800);
		view.SetNeedsLayout();
		view.LayoutIfNeeded();

		UIKit.UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0.0f);
		var ctx = UIKit.UIGraphics.GetCurrentContext();
		// ctx will be null if the width/height of the view is zero
		if (ctx is not null && !TryRender(view, out _))
		{
			// TODO: test/handle this case
		}

		var image = UIGraphics.GetImageFromCurrentImageContext();
		UIGraphics.EndImageContext();

		var mauiImage = new Image() { Source = ImageSource.FromStream(() => image.AsPNG().AsStream()) };
		dotnetBot.Source = mauiImage.Source;
#elif ANDROID
		var view = page.ToPlatformEmbedded(this.Handler!.MauiContext!);
		int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(800, MeasureSpecMode.Exactly);
    	int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(800, MeasureSpecMode.Exactly);
		view.Measure(widthMeasureSpec, heightMeasureSpec);
		view.Layout(0, 0, 800, 800);
		view.LayoutParameters = new ViewGroup.LayoutParams(800, 800);
		// From https://github.com/dotnet/maui/blob/a835905e81d2b8142f40e32e0e2b1601add6eb25/src/Essentials/src/Screenshot/Screenshot.android.cs,
		// No changes.
		
		var bitmap = Render(view);
		Stream destination = new MemoryStream();
		bitmap.Compress(Bitmap.CompressFormat.Png, 100, destination);
		destination.Position = 0;
		var mauiImage = new Image() { Source = ImageSource.FromStream(() => destination) };
		dotnetBot.Source = mauiImage.Source;
#endif
		// var result = await page.CaptureAsync();
	}

#if IOS || MACCATALYST

	static bool TryRender(UIView view, out Exception? error)
	{
		try
		{
			view.DrawViewHierarchy(view.Bounds, afterScreenUpdates: true);

			error = null;
			return true;
		}
		catch (Exception e)
		{
			error = e;
			return false;
		}
	}

	static bool TryRender(CALayer layer, CGContext ctx, bool skipChildren, out Exception? error)
	{
		var visibilitySnapshot = new Dictionary<CALayer, bool>();

		try
		{
			if (skipChildren)
				HideSublayers(layer, visibilitySnapshot);

			layer.RenderInContext(ctx);

			error = null;
			return true;
		}
		catch (Exception e)
		{
			error = e;
			return false;
		}
		finally
		{
			if (skipChildren)
				RestoreSublayers(layer, visibilitySnapshot);
		}
	}

	static void HideSublayers(CALayer layer, Dictionary<CALayer, bool> visibilitySnapshot)
	{
		var sublayers = layer?.Sublayers;
		if (sublayers is null)
			return;

		foreach (var sublayer in sublayers)
		{
			HideSublayers(sublayer, visibilitySnapshot);

			visibilitySnapshot.Add(sublayer, sublayer.Hidden);
			sublayer.Hidden = true;
		}
	}

	static void RestoreSublayers(CALayer layer, Dictionary<CALayer, bool> visibilitySnapshot)
	{
		if (layer.Sublayers == null)
			return;

		foreach (var sublayer in visibilitySnapshot)
		{
			sublayer.Key.Hidden = sublayer.Value;
		}
	}
	
#elif ANDROID
	static Bitmap Render(Android.Views.View view)
	{
		var bitmap = RenderUsingCanvasDrawing(view);

		//if (bitmap == null)
			//bitmap = RenderUsingDrawingCache(view);

		return bitmap;
	}

	static Bitmap RenderUsingCanvasDrawing(Android.Views.View view)
	{
		try
		{
			if (view?.LayoutParameters == null || Bitmap.Config.Argb8888 == null)
				return null;
			var width = view.Width;
			var height = view.Height;

			var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
			if (bitmap == null)
				return null;

			using (var canvas = new Canvas(bitmap))
				view.Draw(canvas);

			return bitmap;
		}
		catch (Exception ex)
		{
			return null;
		}
	}
#endif
}
