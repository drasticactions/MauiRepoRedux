using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ScreenshotTest;

public partial class TestWindow : Window
{
    public TestWindow(Page page)
        : base(page)
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if MACCATALYST
        var view = (UIView)this.TestTitlebar.Handler!.PlatformView!;
#endif
    }
}