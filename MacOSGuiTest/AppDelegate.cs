using CoreAnimation;
using Masonry;

namespace MacOSGuiTest;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	public override void DidFinishLaunching (NSNotification notification)
	{
		var window = new MainWindow ();
		window.Title = "Hello World";
		window.Center ();
		window.MakeKeyAndOrderFront (null);
	}

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}
}

public class MainWindow : NSWindow
{
	private sealed class PostEditor : NSTextView
    {
        public PostEditor()
        {
            this.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            this.Font = NSFont.FromFontName("Helvetica Neue Bold", 18)!;
            this.DrawsBackground = false;
            this.BackgroundColor = NSColor.Clear;
            this.TextContainerInset = new CGSize(10, 10);
        }
    }

    private sealed class PostEditorScrollView : NSScrollView
    {
        private PostEditor postEditor;

        public PostEditorScrollView(PostEditor postEditor)
        {
            this.postEditor = postEditor;
            this.DocumentView = postEditor;
            this.HasVerticalScroller = true;
            this.HasHorizontalScroller = false;
            this.AutohidesScrollers = true;
            this.WantsLayer = true;
            this.BackgroundColor = NSColor.Clear;
            this.Layer!.CornerRadius = 10;
        }
    }

	private PostEditorScrollView postEditor;
	private NSView imageWrapperView;

	public MainWindow ()
		: base (new CGRect (200, 200, 400, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable, NSBackingStore.Buffered, false)
		{
			postEditor = new PostEditorScrollView(new PostEditor());
			this.ContentView = new NSView();
			imageWrapperView= new NSView();
			imageWrapperView.WantsLayer = true;
			var buttonsView = new NSView();
			buttonsView.WantsLayer = true;
			this.ContentView.AddSubview(postEditor);
			this.ContentView.AddSubview(imageWrapperView);
			this.ContentView.AddSubview(buttonsView);
			var buttonOne = new NSButton();
			buttonOne.ControlSize = NSControlSize.Large;
			buttonOne.Image = NSImage.GetSystemSymbol("plus", null);
			var buttonTwo = new NSButton();
			buttonTwo.ControlSize = NSControlSize.Large;
			buttonTwo.Image = NSImage.GetSystemSymbol("minus", null);
			var buttonThree = new NSButton();
			buttonThree.ControlSize = NSControlSize.Large;
			buttonThree.Image = NSImage.GetSystemSymbol("multiply", null);
			buttonsView.AddSubview(buttonOne);
			buttonOne.Activated += (sender, e) =>
			{
				
			};
			buttonsView.AddSubview(buttonTwo);
			buttonTwo.Activated += (sender, e) =>
			{
				
			};
			buttonsView.AddSubview(buttonThree);

			buttonOne.MakeConstraints(make =>
			{
				make.Left.EqualTo(buttonsView).Offset(0);
				make.Top.EqualTo(buttonsView).Offset(10);
				make.Bottom.EqualTo(buttonsView).Offset(-10);
			});

			buttonTwo.MakeConstraints(make =>
			{
				make.Left.EqualTo(buttonOne.Right()).Offset(20);
				make.Top.EqualTo(buttonsView).Offset(10);
				make.Bottom.EqualTo(buttonsView).Offset(-10);
			});

			buttonThree.MakeConstraints(make =>
			{
				make.Right.EqualTo(buttonsView).Offset(0);
				make.Top.EqualTo(buttonsView).Offset(10);
				make.Bottom.EqualTo(buttonsView).Offset(-10);
			});

			this.postEditor.MakeConstraints(make =>
			{
				make.Top.EqualTo(this.ContentView).Offset(20);
				make.Left.EqualTo(this.ContentView).Offset(20);
				make.Right.EqualTo(this.ContentView).Offset(-20);
				make.Height.EqualTo(NSObject.FromObject(100));
			});

			var test = imageWrapperView.MakeConstraints(make =>
			{
				make.Top.EqualTo(this.postEditor.Bottom()).Offset(20);
				make.Left.EqualTo(this.ContentView).Offset(20);
				make.Right.EqualTo(this.ContentView).Offset(-20);
				make.Height.EqualTo(NSObject.FromObject(0));
			});

			buttonsView.MakeConstraints(make =>
			{
				make.Top.EqualTo(imageWrapperView.Bottom()).Offset(20);
				make.Left.EqualTo(this.ContentView).Offset(20);
				make.Right.EqualTo(this.ContentView).Offset(-20);
				make.Bottom.EqualTo(this.ContentView).Offset(-20);
			});
		}

    public override void UpdateConstraintsIfNeeded()
    {
        base.UpdateConstraintsIfNeeded();
    }
}
