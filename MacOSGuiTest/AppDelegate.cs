using CoreAnimation;
using Masonry;

namespace MacOSGuiTest;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
	public override void DidFinishLaunching(NSNotification notification)
	{
		var mainWindow = new MainWindow();
		mainWindow.Title = "Hello World";
		mainWindow.Center();
		mainWindow.MakeKeyAndOrderFront(this);
		// var mainWindowController = new MainWindowController(new CGRect(200, 200, 400, 400));
        //mainWindowController.Window.OrderFront(this);
	}

	public override void WillTerminate(NSNotification notification)
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

	private Constraint height;

	private Constraint left;

	private bool isExpanded = false;

	public MainWindow()
		: base(new CGRect(200, 200, 400, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable, NSBackingStore.Buffered, false)
	{
		postEditor = new PostEditorScrollView(new PostEditor());
		this.ContentView = new NSView();
		imageWrapperView = new NSView();
		imageWrapperView.WantsLayer = true;
		imageWrapperView.Layer.BackgroundColor = NSColor.SystemBlue.CGColor;
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
			// Animate the height constraint
			NSAnimationContext.BeginGrouping();
			NSAnimationContext.CurrentContext.Duration = 0.3;
			NSAnimationContext.CurrentContext.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

			// Toggle between 100 and 300 height
			
			// ((NSLayoutConstraint)heightConstraint.Animator).Constant = isExpanded ? 100 : 300;
			// isExpanded = !isExpanded;
			((Constraint)height.Animator).EqualTo(NSObject.FromObject(isExpanded ? 0 : 300));
			isExpanded = !isExpanded;
			NSAnimationContext.EndGrouping();
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

		imageWrapperView.MakeConstraints(make =>
		{
			make.Top.EqualTo(this.postEditor.Bottom()).Offset(20);
			this.left = make.Left.EqualTo(this.ContentView).Offset(20);
			make.Right.EqualTo(this.ContentView).Offset(-20);
			this.height = make.Height.EqualTo(NSObject.FromObject(0));
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

public class MainWindowController : NSWindowController
{
	private NSView animatedView;
	private NSLayoutConstraint heightConstraint;
	private NSButton toggleButton;
	private bool isExpanded = false;

	public MainWindowController(CGRect contentRect)
		: base(new NSWindow(contentRect, NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Resizable, NSBackingStore.Buffered, false))
	{
		Window.Title = "Constraint Animation Demo";

		// Set up the content view
		var contentView = Window.ContentView;
		contentView.WantsLayer = true;

		// Create the view to animate
		animatedView = new NSView
		{
			WantsLayer = true,
			TranslatesAutoresizingMaskIntoConstraints = false
		};
		animatedView.Layer.BackgroundColor = NSColor.SystemBlue.CGColor;
		contentView.AddSubview(animatedView);

		// Create toggle button
		toggleButton = new NSButton
		{
			Title = "Toggle Height",
			BezelStyle = NSBezelStyle.Rounded,
			TranslatesAutoresizingMaskIntoConstraints = false
		};
		toggleButton.Activated += HandleToggleButton;
		contentView.AddSubview(toggleButton);

		// Set up constraints
		heightConstraint = NSLayoutConstraint.Create(
			animatedView,
			NSLayoutAttribute.Height,
			NSLayoutRelation.Equal,
			null,
			NSLayoutAttribute.NoAttribute,
			1.0f,
			100); // Initial height

		animatedView.AddConstraint(heightConstraint);

		// Center view horizontally
		contentView.AddConstraint(NSLayoutConstraint.Create(
			animatedView,
			NSLayoutAttribute.CenterX,
			NSLayoutRelation.Equal,
			contentView,
			NSLayoutAttribute.CenterX,
			1.0f,
			0));

		// Center view vertically
		contentView.AddConstraint(NSLayoutConstraint.Create(
			animatedView,
			NSLayoutAttribute.CenterY,
			NSLayoutRelation.Equal,
			contentView,
			NSLayoutAttribute.CenterY,
			1.0f,
			0));

		// Set width
		contentView.AddConstraint(NSLayoutConstraint.Create(
			animatedView,
			NSLayoutAttribute.Width,
			NSLayoutRelation.Equal,
			null,
			NSLayoutAttribute.NoAttribute,
			1.0f,
			200));

		// Position button below animated view
		contentView.AddConstraint(NSLayoutConstraint.Create(
			toggleButton,
			NSLayoutAttribute.Top,
			NSLayoutRelation.Equal,
			animatedView,
			NSLayoutAttribute.Bottom,
			1.0f,
			20));

		// Center button horizontally
		contentView.AddConstraint(NSLayoutConstraint.Create(
			toggleButton,
			NSLayoutAttribute.CenterX,
			NSLayoutRelation.Equal,
			contentView,
			NSLayoutAttribute.CenterX,
			1.0f,
			0));
	}

	private void HandleToggleButton(object sender, EventArgs e)
	{
		// Animate the height constraint
		NSAnimationContext.BeginGrouping();
		NSAnimationContext.CurrentContext.Duration = 0.3;
		NSAnimationContext.CurrentContext.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

		// Toggle between 100 and 300 height
		((NSLayoutConstraint)heightConstraint.Animator).Constant = isExpanded ? 100 : 300;
		isExpanded = !isExpanded;

		NSAnimationContext.EndGrouping();
	}
}
