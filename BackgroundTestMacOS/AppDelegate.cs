using System.Runtime.InteropServices;
using ObjCRuntime;

namespace BackgroundTestMacOS;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	private NSWindow _window;
	
	public override void DidFinishLaunching (NSNotification notification)
	{
		// Insert code here to initialize your application
		_window = new NSWindow (new CGRect (200, 200, 400, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable, NSBackingStore.Buffered, false);
		_window.Title = "Hello World";
		_window.TitleVisibility = NSWindowTitleVisibility.Hidden;
		_window.TitlebarAppearsTransparent = true;

				
		var visualEffectView = new NSVisualEffectView(_window.ContentView!.Bounds)
		{
			Material = NSVisualEffectMaterial.UnderWindowBackground,
			BlendingMode = NSVisualEffectBlendingMode.BehindWindow,
			State = NSVisualEffectState.FollowsWindowActiveState,
			AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
			WantsLayer = true
		};
		
		var contentView = _window.ContentView!;
		contentView.AddSubview(new NSView(), NSWindowOrderingMode.Below, null);	

		var item = Runtime.GetNSObject(AppKit.Call("NSVisualEffectView", "alloc"))!;
		ObjC.Call(item.Handle, "initWithFrame:", _window.ContentView!.Bounds);
		ObjC.Call(item.Handle, "setMaterial:", 21);
		ObjC.Call(item.Handle, "setBlendingMode:", 0);
		ObjC.Call(item.Handle, "setState:", 0);
		ObjC.Call(item.Handle, "setAutoresizingMask:", 18);
		ObjC.Call(item.Handle, "setWantsLayer:", true);
		
		ObjC.Call(_window.Handle, "setContentView:", item.Handle);
		
		_window.MakeKeyAndOrderFront(this);
	}

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
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