using AppKit;

namespace TabBarTestCatalyst;

public class ToolbarDelegate : NSToolbarDelegate
{
    private const string Settings = "Settings";

    /// <inheritdoc/>
    public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
    {
        NSToolbarItem toolbarItem = new NSToolbarItem(itemIdentifier);
        if (itemIdentifier == Settings)
        {
            toolbarItem.UIImage = UIImage.GetSystemImage("gearshape");
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
            Settings,
        };
    }
}