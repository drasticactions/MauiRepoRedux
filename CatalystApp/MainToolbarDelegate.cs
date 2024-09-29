using AppKit;
using Foundation;
using UIKit;

namespace CatalystApp;

public class MainToolbarDelegate : AppKit.NSToolbarDelegate
{
    private const string Settings = "Settings";

    /// <inheritdoc/>
    public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
    {
        NSToolbarItem toolbarItem = new NSToolbarItem(itemIdentifier);
        if (itemIdentifier == Settings)
        {
            toolbarItem.UIImage = UIImage.GetSystemImage("gearshape");
            toolbarItem.Action = new ObjCRuntime.Selector("buttonClickAction:");
            toolbarItem.Target = this;
            toolbarItem.Label = "Settings";
            toolbarItem.Enabled = true;
        }

        return toolbarItem;
    }

    /// <inheritdoc/>
    public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
    {
        return new string[]
        {
            Settings,
        };
    }

    /// <inheritdoc/>
    public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
    {
        return new string[]
        {
            "NSToolbarPrimarySidebarTrackingSeparatorItem",
            "NSToolbarSupplementarySidebarTrackingSeparatorItem",
            NSToolbar.NSToolbarFlexibleSpaceItemIdentifier,
            Settings,
        };
    }

    [Export("buttonClickAction:")]
    public async void ButtonClickAction(NSObject sender)
    {
    }
}