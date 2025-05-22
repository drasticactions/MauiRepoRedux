using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.iOS;

namespace TestApp;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
	public override UIWindow? Window
	{
		get;
		set;
	}

	public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow(UIScreen.MainScreen.Bounds);

		var avaloniaBuilder = AppBuilder.Configure<AvaloniaApp>();
		avaloniaBuilder.UseiOS();
		avaloniaBuilder.SetupWithoutStarting();

		// create a UIViewController with a single UILabel
		var vc = new UINavigationController(new TestUIViewController());
		//var vc = new UIViewController();
		var testview = new TestControl();
		var av = new AvaloniaView();
		av.Content = testview;
		vc.View!.AddSubview(av);
		av.TranslatesAutoresizingMaskIntoConstraints = false;
		av.LeftAnchor.ConstraintEqualTo(vc.View.LeftAnchor).Active = true;
		av.TopAnchor.ConstraintEqualTo(vc.View.TopAnchor).Active = true;
		av.WidthAnchor.ConstraintEqualTo(200).Active = true;
		av.HeightAnchor.ConstraintEqualTo(200).Active = true;
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible();

		return true;
	}
}

public class TestUIViewController : UIViewController
{
	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		View.BackgroundColor = UIColor.White;

		// Add button to navigate to the new TestUIViewController
		var button = new UIButton(UIButtonType.System);
		button.SetTitle("Push me!", UIControlState.Normal);
		button.TouchUpInside += (sender, e) =>
		{
			var testViewController = new TestUIViewController();
			NavigationController?.PushViewController(testViewController, true);
		};

		this.View!.AddSubview(button);
		button.TranslatesAutoresizingMaskIntoConstraints = false;
		button.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
		button.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
		button.WidthAnchor.ConstraintEqualTo(200).Active = true;
		button.HeightAnchor.ConstraintEqualTo(50).Active = true;
	}
}

