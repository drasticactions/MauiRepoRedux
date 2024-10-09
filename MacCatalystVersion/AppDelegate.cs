using System.Runtime.InteropServices;

namespace MacCatalystVersion;

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
		var isMacCatalyst = OperatingSystem.IsMacCatalyst();
		var osVersion = Environment.OSVersion.VersionString;
		var is18 = OperatingSystem.IsMacCatalystVersionAtLeast(18, 0, -1);
		var vc = new UIViewController ();
		IntPtr processInfo = libobjc.objc_msgSend(libobjc.objc_getClass("NSProcessInfo"), libobjc.sel_getUid("processInfo"));
		var objcVersion = libobjc.get_operatingSystemVersion(processInfo,libobjc.sel_getUid("operatingSystemVersion"));
		var test = NSProcessInfo.ProcessInfo.OperatingSystemVersion;
		var testAgain = Interop.Sys.iOSSupportVersion();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = $"Hello, Mac Catalyst! Is Mac Catalyst: {isMacCatalyst} Catalyst Version: {osVersion}, OS Version: {objcVersion}, iOS Support Version: {testAgain} Is 18.0+: {is18}",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}

	internal static partial class libobjc
	{
		internal const string libobjc_name = "/usr/lib/libobjc.dylib";
		
		[DllImport(libobjc_name, EntryPoint = "objc_msgSend")]
		public static extern NSOperatingSystemVersion get_operatingSystemVersion(IntPtr basePtr, IntPtr selector);
		
		[DllImport(libobjc_name)]
		public static extern IntPtr objc_getClass(string className);
		[DllImport(libobjc_name)]
		public static extern IntPtr sel_getUid(string selector);
		[DllImport(libobjc_name)]
		public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector);
	}
	
	public static class Interop
	{
		public static class Sys
		{
			[DllImport("libSystem.Native", EntryPoint = "SystemNative_iOSSupportVersion")]
			public static extern string iOSSupportVersion();
		}
	}
}
