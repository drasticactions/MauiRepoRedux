using SDWebImage;

namespace SDWebImageExample;

public class MyCustomTableViewCell : UITableViewCell
    {
        public UILabel? CustomTextLabel { get; private set; }
        public SDAnimatedImageView? CustomImageView { get; private set; }

        public MyCustomTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public MyCustomTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            CustomImageView = new SDAnimatedImageView();
            CustomImageView.Frame = new CoreGraphics.CGRect(12.0, 12.0, 60.0, 60.0);
            ContentView.AddSubview(CustomImageView);
            
            CustomTextLabel = new UILabel(new CoreGraphics.CGRect(100.0, 12.0, 200, 20.0));
            ContentView.AddSubview(CustomTextLabel);
            
            CustomImageView.ClipsToBounds = true;
            CustomImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
        }
    }