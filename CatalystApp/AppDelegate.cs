using AppKit;
using AppleShared;
using Foundation;
using UIKit;

namespace CatalystApp;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new MainViewController ();
		Window.RootViewController = vc;

		var windowScene = Window.WindowScene;

		if (windowScene is not null)
		{
			windowScene.Titlebar!.TitleVisibility = UITitlebarTitleVisibility.Visible;
			var nsToolbar = new NSToolbar();
			nsToolbar.Delegate = new MainToolbarDelegate();
			windowScene.Titlebar!.Toolbar = nsToolbar;
		}
		
		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}
