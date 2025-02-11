namespace CatalystTest;

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
		var vc = new UIViewController ();

		var label = new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			AutoresizingMask = UIViewAutoresizing.All,
		};

		var htmlCodeText = @"<code><label for=""name"">Name (4 to 8 characters):</label><input type=""text"" id=""name"" name=""name"" required minlength=""4"" maxlength=""8"" size=""10"" /></code>";
		var nsattributedText = new NSAttributedStringDocumentAttributes {
			DocumentType = NSDocumentType.HTML,
			CharacterEncoding = NSStringEncoding.UTF8
		};

		var error = new NSError ();

		var attributedString = new NSAttributedString (NSData.FromString (htmlCodeText), nsattributedText, ref error);

		label.AttributedText = attributedString;
		
		vc.View!.AddSubview (label);


		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}
