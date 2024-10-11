using Foundation;
using ObjCRuntime;
using UIKit;

namespace BackgroundTestMaui;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	protected override async void OnHandlerChanged()
	{
		base.OnHandlerChanged();

		var test = (UIView)this.Handler.PlatformView;
		var parent = test.FindViewController();
		parent.View.BackgroundColor = UIColor.Clear;
		parent.View.Opaque = false;
		parent.View.Subviews[0].BackgroundColor = UIColor.Clear;
	}

	private async void Button_OnClicked(object? sender, EventArgs e)
	{
		var uiview = (UIView)this.Handler.PlatformView;
		var uiviewController = uiview.FindViewController();
		var views = new List<UIView>();
		uiviewController.View = new UIView();
		var window = (UIWindow)this.GetParentWindow().Handler.PlatformView;
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (window!.Frame) {
			BackgroundColor = UIColor.Clear,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, Mac Catalyst!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		window.RootViewController = vc;
		// uiviewController.View!.AddSubview (new UILabel (window!.Frame) {
		// 	BackgroundColor = UIColor.Clear,
		// 	TextAlignment = UITextAlignment.Center,
		// 	Text = "Hello, Mac Catalyst!",
		// 	AutoresizingMask = UIViewAutoresizing.All,
		// });
		//(uiviewController.View.Subviews[0]).RemoveFromSuperview();
		//(uiviewController.View.Subviews[1]).RemoveFromSuperview();
		var item = Runtime.GetNSObject(AppKit.Call("NSVisualEffectView", "alloc"))!;
		ObjC.Call(item.Handle, "initWithFrame:", UIScreen.MainScreen.Bounds);
		ObjC.Call(item.Handle, "setMaterial:", 21);
		ObjC.Call(item.Handle, "setBlendingMode:", 0);
		ObjC.Call(item.Handle, "setState:", 1);
		ObjC.Call(item.Handle, "setAutoresizingMask:", 18);
		ObjC.Call(item.Handle, "setWantsLayer:", true);
		
		var uinsWindow = await window.ToUINSWindowAsync();
		var attachedWindow = uinsWindow.NSWindow.PerformSelector(new Selector("attachedWindow"))!;
		// ObjC.Call(attachedWindow.Handle, "setContentViewController:", IntPtr.Zero);
		ObjC.Call(attachedWindow.Handle, "setTitlebarAppearsTransparent:", true);
		//ObjC.Call(attachedWindow.Handle, "setContentView:", item.Handle);
		var contentView = attachedWindow.PerformSelector(new Selector("contentView"))!;
		ObjC.Call(contentView.Handle, "addSubview:positioned:relativeTo:", item.Handle, -1, IntPtr.Zero);
	}
}

public static class UIViewExtensions
{
	public static UIViewController FindViewController(this UIView view)
	{
		UIResponder nextResponder = view.NextResponder;
        
		if (nextResponder is UIViewController controller)
		{
			return controller;
		}
		else if (nextResponder is UIView nextView)
		{
			return nextView.FindViewController();
		}
		else
		{
			return null;
		}
	}

	public static void GetAllUIViews(this UIView view, List<UIView> views)
	{
		if (!view.Subviews.Any())
		{
			return;
		}
		
		views.AddRange(view.Subviews);
		foreach (var subview in view.Subviews)
		{
			subview.GetAllUIViews(views);
		}
	}
}

