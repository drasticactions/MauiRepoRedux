using System.Runtime.InteropServices;
using ObjCRuntime;

namespace BackgroundTest;

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
		var windowScene = this.Window.WindowScene;
		
		if (windowScene?.Titlebar != null)
		{
			windowScene.Titlebar.TitleVisibility = UITitlebarTitleVisibility.Hidden;
		}
		
		var item = Runtime.GetNSObject(AppKit.Call("NSVisualEffectView", "alloc"))!;
		ObjC.Call(item.Handle, "initWithFrame:", UIScreen.MainScreen.Bounds);
		ObjC.Call(item.Handle, "setMaterial:", 21);
		ObjC.Call(item.Handle, "setBlendingMode:", 0);
		ObjC.Call(item.Handle, "setState:", 0);
		ObjC.Call(item.Handle, "setAutoresizingMask:", 18);
		ObjC.Call(item.Handle, "setWantsLayer:", true);

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.Clear,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, Mac Catalyst!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;
		
		
		
		this.InvokeOnMainThread(async () =>
		{
			var nsWindow = await Window.ToUINSWindowAsync();
			if (nsWindow is null)
			{
				return;
			}
			var contentView = nsWindow.NSWindow.PerformSelector(new Selector("contentView"))!;
			ObjC.Call(contentView.Handle, "addSubview:positioned:relativeTo:", item.Handle, -1, IntPtr.Zero);
			//ObjC.Call(nsWindow.NSWindow.Handle, "setContentView:", item.Handle);
		});
		
		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

/// <summary>
    /// NSApplication Helpers.
    /// </summary>
    public static class NSApplication
    {
        /// <summary>
        /// Gets the shared NSApplication instance.
        /// </summary>
        /// <returns>NSObject.</returns>
        internal static NSObject GetSharedApplication()
        {
            var nsApplication = Runtime.GetNSObject(Class.GetHandle("NSApplication"))!;
            var sharedApplication = nsApplication.PerformSelector(new Selector("sharedApplication"))!;
            return sharedApplication;
        }
    }

internal static class Extensions
{
	/// <summary>
	/// Get NSWindow from UIWindow.
	/// </summary>
	/// <param name="window">UIWindow.</param>
	/// <returns><see cref="UINSWindow"/>.</returns>
	internal static async Task<UINSWindow?> ToUINSWindowAsync(this UIWindow window)
	{
		var nsWindow = await window.GetNSWindowFromUIWindowAsync();
		return nsWindow is null ? null : new UINSWindow(nsWindow, window);
	}
	
	/// <summary>
	/// Get NSWindow from UIWindow.
	/// </summary>
	/// <param name="window">UIWindow.</param>
	/// <returns>NSWindow as NSObject.</returns>
	internal static async Task<NSObject?> GetNSWindowFromUIWindowAsync(this UIWindow window)
	{
		if (window is null)
		{
			return null;
		}

		var sharedApplication = NSApplication.GetSharedApplication();

		var applicationDelegate = sharedApplication.PerformSelector(new Selector("delegate"));
		if (applicationDelegate is null)
		{
			return null;
		}

		return await GetNSWindowAsync(window, applicationDelegate);
	}

	internal static async Task<NSObject?> GetNSWindowAsync(UIWindow window, NSObject applicationDelegate)
	{
		var nsWindowHandle = ObjC.Call(applicationDelegate.Handle, "hostWindowForUIWindow:", window.Handle);
		var nsWindow = Runtime.GetNSObject<NSObject>(nsWindowHandle);
		if (nsWindow is null)
		{
			await Task.Delay(500);
			return await GetNSWindowAsync(window, applicationDelegate);
		}

		return nsWindow;
	}
}

 /// <summary>
    /// UINSWindow is the underlying NSWindow contained within a UIWindow.
    /// This is used to poke at the underlying Window element to get access to additional features.
    /// </summary>
    internal class UINSWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UINSWindow"/> class.
        /// </summary>
        /// <param name="nsWindow">The NSWindow instance.</param>
        /// <param name="uiWindow">The UIWindow.</param>
        public UINSWindow(NSObject nsWindow, UIWindow uiWindow)
        {
            this.NSWindow = nsWindow;
            this.UIWindow = uiWindow;
        }

        /// <summary>
        /// Gets the NSWindow of the given UIWindow.
        /// </summary>
        public NSObject NSWindow { get; }

        /// <summary>
        /// Gets the UIWindow.
        /// </summary>
        public UIWindow UIWindow { get; }

        /// <summary>
        /// Gets the internal NSWindow Frame.
        /// </summary>
        public CGRect? Frame
        {
            get
            {
                var attachedWindow = this.NSWindow.ValueForKey(new Foundation.NSString("attachedWindow"));

                if (attachedWindow is null)
                {
                    return null;
                }

                var windowFrame = (NSValue)attachedWindow.ValueForKey(new Foundation.NSString("frame"));

                return windowFrame.CGRectValue;
            }
        }

        /// <summary>
        /// Sets the given NSWindow Frame.
        /// </summary>
        /// <param name="rect">The size of the new frame.</param>
        /// <param name="display">Should display the frame.</param>
        /// <param name="animate">Should animate the change.</param>
        public void SetFrame(CGRect rect, bool display = true, bool animate = true)
        {
            var attachedWindow = this.NSWindow.ValueForKey(new Foundation.NSString("attachedWindow"));

            if (attachedWindow is null)
            {
                return;
            }

            var windowFrame = (NSValue)attachedWindow.ValueForKey(new Foundation.NSString("frame"));

            ObjC.Call(attachedWindow.Handle, "setFrame:display:animate:", rect, display, animate);
        }
    }

public class BlurredBackgroundRootViewController : UIViewController
{
    private readonly UISplitViewController _backgroundSplitViewController;

    public BlurredBackgroundRootViewController()
    {
        // Initialize UISplitViewController with double column style
        _backgroundSplitViewController = new UISplitViewController(UISplitViewControllerStyle.DoubleColumn)
        {
            OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark,
            PreferredPrimaryColumnWidthFraction = 0.5f,
            PrimaryBackgroundStyle = UISplitViewControllerBackgroundStyle.Sidebar
        };

        var a = new UIViewController();
        var b = new UIViewController();

        // Set initial view controllers of the SplitViewController
        _backgroundSplitViewController.ViewControllers = new UIViewController[] { a, b };

        // Hide navigation bar for both controllers
        a.NavigationController?.SetNavigationBarHidden(true, false);
        a.View.Hidden = true;

        b.NavigationController?.SetNavigationBarHidden(true, false);
        b.View.Hidden = true;

        // Add child SplitViewController and its view
        AddChildViewController(_backgroundSplitViewController);
        View.AddSubview(_backgroundSplitViewController.View);
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();

        // Set primary column width to match the entire width of the root view
        _backgroundSplitViewController.MinimumPrimaryColumnWidth = View.Bounds.Width;
        _backgroundSplitViewController.MaximumPrimaryColumnWidth = View.Bounds.Width;

        // Adjust the frame of the SplitViewController's view
        _backgroundSplitViewController.View.Frame = new CGRect(0, 0, View.Bounds.Width * 2, View.Bounds.Height);
    }

    // Override the default constructor with NSCoder to throw an exception
    public BlurredBackgroundRootViewController(IntPtr handle) : base(handle)
    {
        throw new NotImplementedException("init(coder:) has not been implemented");
    }
}

internal static class AppKit
{
	private const string AppKitFramework = "/System/Library/Frameworks/AppKit.framework/AppKit";

	[DllImport(AppKitFramework, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetClass(string name);

	[DllImport(AppKitFramework, EntryPoint = "objc_getProtocol", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetProtocol(string name);

	public static IntPtr Call(string id, string sel)
	{
		return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel));
	}

	public static IntPtr Call(string id, string sel, double a, double b, double c, double d)
	{
		return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a, b, c, d);
	}

	public static IntPtr Call(string id, string sel, IntPtr a)
	{
		return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a);
	}
}

internal static class ObjC
    {
        private const string ObjCLib = "/usr/lib/libobjc.dylib";

        [DllImport(ObjCLib, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetClass(string name);

        [DllImport(ObjCLib, EntryPoint = "objc_allocateClassPair", CharSet = CharSet.Ansi)]
        public static extern IntPtr AllocateClassPair(IntPtr superclass, string name, IntPtr extraBytes);

        [DllImport(ObjCLib, EntryPoint = "objc_registerClassPair")]
        public static extern void RegisterClassPair(IntPtr cls);

        [DllImport(ObjCLib, EntryPoint = "class_addProtocol")]
        public static extern bool AddProtocol(IntPtr cls, IntPtr protocol);

        [DllImport(ObjCLib, EntryPoint = "objc_getProtocol", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProtocol(string name);

        [DllImport(ObjCLib, EntryPoint = "class_addMethod", CharSet = CharSet.Ansi)]
        public static extern bool AddMethod(IntPtr cls, IntPtr name, Delegate imp, string types);

        [DllImport(ObjCLib, EntryPoint = "class_addIvar", CharSet = CharSet.Ansi)]
        public static extern bool AddVariable(IntPtr cls, string name, IntPtr size, byte alignment, string types);

        [DllImport(ObjCLib, EntryPoint = "class_getInstanceVariable", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetVariable(IntPtr cls, string name);

        [DllImport(ObjCLib, EntryPoint = "object_getIvar")]
        public static extern IntPtr GetVariableValue(IntPtr obj, IntPtr ivar);

        [DllImport(ObjCLib, EntryPoint = "object_setIvar")]
        public static extern IntPtr SetVariableValue(IntPtr obj, IntPtr ivar, IntPtr value);

        [DllImport(ObjCLib, EntryPoint = "sel_registerName", CharSet = CharSet.Ansi)]
        public static extern IntPtr RegisterName(string? name);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, UIntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b, IntPtr c);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b, IntPtr c, IntPtr d);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, UIntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr[] a, IntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, [MarshalAs(UnmanagedType.I1)] bool value);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, double value);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, double a, double b, double c, double d);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect rect, IntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect a, bool b, bool c);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect rect, UIntPtr a, UIntPtr b, [MarshalAs(UnmanagedType.I1)] bool c);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGSize size);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect rect);

        public static IntPtr Call(IntPtr id, string sel)
        {
            return SendMessage(id, RegisterName(sel));
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, UIntPtr a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, double a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, bool a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }
        
        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b)
        {
	        return SendMessage(id, RegisterName(sel), a, b);
        }
        
        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b, IntPtr c)
        {
            return SendMessage(id, RegisterName(sel), a, b, c);
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b, IntPtr c, IntPtr d)
        {
            return SendMessage(id, RegisterName(sel), a, b, c, d);
        }

        public static IntPtr Call(IntPtr id, string sel, CGRect rect, IntPtr a)
        {
            return SendMessage(id, RegisterName(sel), rect, a);
        }

        public static IntPtr Call(IntPtr id, string sel, CGRect rect, bool a, bool b)
        {
            return SendMessage(id, RegisterName(sel), rect, a, b);
        }

        public static IntPtr Call(IntPtr id, string sel, CGSize size)
        {
            return SendMessage(id, RegisterName(sel), size);
        }

        public static IntPtr Call(IntPtr id, string sel, CGRect rect)
        {
            return SendMessage(id, RegisterName(sel), rect);
        }
    }
