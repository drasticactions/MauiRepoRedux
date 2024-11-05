using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drastic.TrayWindow;
using UIKit;

namespace ScreenshotTest;

public partial class TestWindow : Window
{
    public TestWindow(Page page)
        : base(page)
    {
        InitializeComponent();
    }

    private UINSWindow? window;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        Task.Run(async () =>
        {
            var platformWindow = (UIWindow)this.Handler.PlatformView;
            this.window = await platformWindow.ToUINSWindowAsync();
        });
#if MACCATALYST
        var view = (UIView)this.TestTitlebar.Handler!.PlatformView!;
        var tapGesture = new UILongPressGestureRecognizer((gestureRecognizer) => {
            if (this.window is not null)
            {
                this.window.PerformWindowDrag();
            }
        });
        tapGesture.MinimumPressDuration = 0.0;
        view.AddGestureRecognizer(tapGesture);
#endif
    }
}