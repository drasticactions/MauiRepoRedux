using Masonry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class MainViewController : UIViewController
    {
        public MainViewController()
        {
            HotReloadService.UpdateApplicationEvent += OnHotReload;
        }

        private void OnHotReload(Type[]? obj)
        {
            this.InvokeOnMainThread(ViewDidLoad);
        }

        public override void ViewDidLoad()
        {
            this.View = new UIView();
            Build();
        }

        private void Build()
        {
            this.View!.BackgroundColor = UIColor.SystemBackground;
            var button = UIButton.FromType(UIButtonType.System);
            var uiButtonConfiguration = UIButtonConfiguration.FilledButtonConfiguration;
            button.Configuration = uiButtonConfiguration;
            button.SetTitle("Click me", UIControlState.Normal);
            button.TouchUpInside += (sender, e) =>
            {
                //var alert = UIAlertController.Create("Hello", "Hello, World!", UIAlertControllerStyle.Alert);
                //alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //this.PresentViewController(alert, true, null);
            };

            this.View.AddSubview(button);
            button.MakeConstraints((make) =>
            {
                make.CenterX.EqualTo(this.View.CenterX());
                make.CenterY.EqualTo(this.View.CenterY());
            });
        }
    }
}
