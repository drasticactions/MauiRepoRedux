using SDWebImage;

namespace SDWebImageExample;

public sealed class SampleViewController : UIViewController
{
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        this.View!.BackgroundColor = UIColor.SystemBackground;

        // UI is a UIProgressView, followed by the UIImageView, followed by a UIButton to download a new image
        // Into the UIImageView

        var progressView = new UIProgressView();
        var imageView = new UIImageView() { BackgroundColor = UIColor.Red, ContentMode = UIViewContentMode.ScaleAspectFit };
        var button = new UIButton(UIButtonType.System);
        button.SetTitle("Download Image", UIControlState.Normal);

        // Set up the progress view
        var progressBlock = new SDImageLoaderProgressBlock((receivedSize, expectedSize, uri) =>
        {
            this.InvokeOnMainThread(() =>
            {
                if (expectedSize > 0)
                {
                    progressView.SetProgress((float)receivedSize / expectedSize, true);
                }
            });
        });

        var externalCompletionBlock = new SDExternalCompletionBlock((image, error, cacheType, url) =>
        {
            if (error != null)
            {
                Console.WriteLine($"Error: {error.LocalizedDescription}");
            }
            else
            {
                Console.WriteLine("Image downloaded successfully");
            }
        });

        button.TouchUpInside += (sender, e) =>
        {
            this.InvokeOnMainThread(() =>
            {
                var url = new NSUrl("https://cdn.bsky.app/img/feed_fullsize/plain/did:plc:lifomyav2alrumoneau25i26/bafkreiegorit5icfzfobjy7alahsqx3o4rcxqf3qrpcb6h76enhvsd2wlu@jpeg");
                imageView.SetImageWithURL(url, null, SDWebImageOptions.ProgressiveLoad, progressBlock, externalCompletionBlock);
            });
        };

        // Add the views to the main view
        this.View!.AddSubviews(progressView, imageView, button);

        progressView.TranslatesAutoresizingMaskIntoConstraints = false;
        imageView.TranslatesAutoresizingMaskIntoConstraints = false;
        button.TranslatesAutoresizingMaskIntoConstraints = false;

        // Set up constraints or frames for the views
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            progressView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20),
            progressView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
            progressView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20),

            imageView.TopAnchor.ConstraintEqualTo(progressView.BottomAnchor, 20),
            imageView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
            imageView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20),
            imageView.HeightAnchor.ConstraintEqualTo(200),

            button.TopAnchor.ConstraintEqualTo(imageView.BottomAnchor, 20),
            button.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor)
        });
    }
}