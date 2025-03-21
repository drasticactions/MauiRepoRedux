using SDWebImage;

namespace SDWebImageExample;

public sealed class MasterViewController : UITableViewController
{
    private List<string> _objects;
    private const string CellIdentifier = "Cell";
    private UIImage _placeholderImage;

    public MasterViewController()
    {
        // // HTTP NTLM auth example
        //     // Add your NTLM image url to the array below and replace the credentials
        //     SDWebImageDownloader.SharedDownloader.Config.Username = "httpwatch";
        //     SDWebImageDownloader.SharedDownloader.Config.Password = "httpwatch01";
        //     SDWebImageDownloader.SharedDownloader.SetValue(new NSString("SDWebImage Demo"), "AppName");
        //     SDWebImageDownloader.SharedDownloader.Config.ExecutionOrder = SDWebImageDownloaderExecutionOrder.LIFOExecutionOrder;

            _objects = new List<string>
            {
                "https://raw.githubusercontent.com/CloudlessMoon/SuperResources/master/Images/HEIC/TestHDR1.heic",
                "https://raw.githubusercontent.com/CloudlessMoon/SuperResources/master/Images/JPG/TestHDR1.JPG",
                "https://raw.githubusercontent.com/CloudlessMoon/SuperResources/master/Images/JPG/TestHDR2.JPG",
                "http://www.httpwatch.com/httpgallery/authentication/authenticatedimage/default.aspx?0.35786508303135633",
                "http://assets.sbnation.com/assets/2512203/dogflops.gif",
                "https://raw.githubusercontent.com/liyong03/YLGIFImage/master/YLGIFImageDemo/YLGIFImageDemo/joy.gif",
                "http://apng.onevcat.com/assets/elephant.png",
                "http://www.ioncannon.net/wp-content/uploads/2011/06/test2.webp",
                "http://www.ioncannon.net/wp-content/uploads/2011/06/test9.webp",
                "http://littlesvr.ca/apng/images/SteamEngine.webp",
                "http://littlesvr.ca/apng/images/world-cup-2014-42.webp",
                "https://isparta.github.io/compare-webp/image/gif_webp/webp/2.webp",
                "https://nokiatech.github.io/heif/content/images/ski_jump_1440x960.heic",
                "https://nokiatech.github.io/heif/content/image_sequences/starfield_animation.heic",
                "https://s2.ax1x.com/2019/11/01/KHYIgJ.gif",
                "https://raw.githubusercontent.com/icons8/flat-color-icons/master/pdf/stack_of_photos.pdf",
                "https://nr-platform.s3.amazonaws.com/uploads/platform/published_extension/branding_icon/275/AmazonS3.png",
                "https://res.cloudinary.com/dwpjzbyux/raw/upload/v1666474070/RawDemo/raw_vebed5.NEF",
                "https://placehold.co/200x200.jpg",
            };

        for (int i = 1; i < 25; i++)
        {
            // From http://r0k.us/graphics/kodak/, 768x512 resolution, 24 bit depth PNG
            _objects.Add($"http://r0k.us/graphics/kodak/kodak/kodim{i:00}.png");
        }

        _placeholderImage = UIImage.GetSystemImage("photo")!;
        NavigationItem.RightBarButtonItem = new UIBarButtonItem( "Clear Cache", UIBarButtonItemStyle.Plain, (sender, e) =>
        {
            SDWebImageManager.SharedManager.ImageCache.ClearMemory();
            SDWebImageManager.SharedManager.ImageCache.ClearDiskOnCompletion(null);
        });
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell(CellIdentifier) as MyCustomTableViewCell;
        
        if (cell == null)
        {
            cell = new MyCustomTableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            //cell.CustomImageView.SetImage(SDWebImageTransition.FadeTransition);
            //cell.CustomImageView.SetIndicator(SDWebImageActivityIndicator.GrayIndicator);
        }
        
        cell.CustomTextLabel!.Text = $"Image #{indexPath.Row}";
        
        var weakImageView = new WeakReference<SDAnimatedImageView>(cell.CustomImageView);
        
        cell.CustomImageView.SetImageWithURL(
            url: new NSUrl(_objects[indexPath.Row]),
            placeholder: _placeholderImage,
            options: 0,
            context: null,
            progressBlock: null,
            completedBlock: null);
        
        return cell;
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        return _objects.Count;
    }

    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
        return 80;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        this.Title = "SDWebImage Example";
        this.TableView.RegisterClassForCellReuse(typeof(MyCustomTableViewCell), nameof(MyCustomTableViewCell));
    }
}