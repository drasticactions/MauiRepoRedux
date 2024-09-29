using Foundation;
using MauiShared;
using Microsoft.Maui.Controls.Embedding;
using UIKit;

namespace AppleShared;

public sealed class MainViewController : UISplitViewController
{
    private SidebarViewController sidebar;
    private UIViewController splitView;
    private UIViewController contentView;
    private readonly MauiContext _mauiContext;
    
    public MainViewController()
        : base(UISplitViewControllerStyle.TripleColumn)
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiEmbeddedApp<Microsoft.Maui.Controls.Application>();
        MauiApp mauiApp = builder.Build();
        
        this._mauiContext = new MauiContext(mauiApp.Services);
        
        this.sidebar = new SidebarViewController();
        this.splitView = new SplitViewController(this._mauiContext);
        this.contentView = new ContentViewController(this._mauiContext);
        this.SetViewController(this.sidebar, UISplitViewControllerColumn.Primary);
        this.SetViewController(this.contentView, UISplitViewControllerColumn.Secondary);
        this.SetViewController(this.splitView, UISplitViewControllerColumn.Supplementary);
        this.PreferredDisplayMode = UISplitViewControllerDisplayMode.TwoBesideSecondary;
        this.PreferredPrimaryColumnWidth = 245f;
        this.PrimaryBackgroundStyle = UISplitViewControllerBackgroundStyle.Sidebar;
    }
    
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        this.ShowColumn(UISplitViewControllerColumn.Primary);
    }
}

public sealed class SplitViewController : UIViewController
{
    private readonly MauiContext _mauiContext;
    private TestListPage mainPage;
    
    public SplitViewController(MauiContext mauiContext)
    {
        this.View!.BackgroundColor = UIColor.SystemBackground;
        this._mauiContext = mauiContext;
        this.mainPage = new TestListPage();
        var content = this.mainPage.ToPlatformEmbedded(this._mauiContext);
        this.View!.AddSubview(content);
        content.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            content.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
            content.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
            content.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            content.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
        });
    }
}

public sealed class ContentViewController : UIViewController
{
    private readonly MauiContext _mauiContext;
    private MainPage mainPage;
    
    public ContentViewController(MauiContext mauiContext)
    {
        this._mauiContext = mauiContext;
        this.mainPage = new MainPage();
        var content = this.mainPage.ToPlatformEmbedded(this._mauiContext);
        this.View!.BackgroundColor = UIColor.SystemBackground;
        this.View!.AddSubview(content);
        content.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            content.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
            content.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
            content.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            content.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
        });
        
    }
}

public sealed class SidebarViewController : UIViewController, IUICollectionViewDelegate
{
    private UICollectionView collectionView;
    private UICollectionViewDiffableDataSource<NSString, SidebarItem> dataSource;
    private readonly NSString foldersSectionIdentifier = new NSString(Guid.NewGuid().ToString());
    
    public SidebarViewController()
    {
        this.collectionView = new UICollectionView(this.View!.Bounds, this.CreateLayout());
        this.collectionView.Delegate = this;
        
        this.dataSource = this.ConfigureDataSource();
        
        this.View.AddSubview(this.collectionView);
        
        this.collectionView.TranslatesAutoresizingMaskIntoConstraints = false;

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.collectionView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
            this.collectionView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
            this.collectionView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            this.collectionView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
        });
        
        this.GenerateItems();
    }

    private void GenerateItems(bool animated = false)
    {
        var snapshot = new NSDiffableDataSourceSectionSnapshot<SidebarItem>();
        var header = new SidebarItem() { Title = "Main", SidebarItemType = SidebarItemType.Header};
        snapshot.AppendItems(new[] { header } );
        snapshot.ExpandItems(new[] { header });
        var listItems = new List<SidebarItem>();
        for (int i = 0; i < 10; i++)
        {
            listItems.Add(new SidebarItem() { Title = $"Item {i}", SidebarItemType = SidebarItemType.Item });
        }
        snapshot.AppendItems(listItems.ToArray(), header);
        
        this.dataSource!.ApplySnapshot(snapshot, this.foldersSectionIdentifier, animated);
    }
    
    private UICollectionViewLayout CreateLayout()
    {
        return new UICollectionViewCompositionalLayout((sectionIndex, layoutEnvironment) =>
        {
            var configuration = new UICollectionLayoutListConfiguration(UICollectionLayoutListAppearance.Sidebar);
            configuration.ShowsSeparators = false;
            configuration.HeaderMode = UICollectionLayoutListHeaderMode.None;
            return NSCollectionLayoutSection.GetSection(configuration, layoutEnvironment);
        });
    }
    
    private UICollectionViewDiffableDataSource<NSString, SidebarItem> ConfigureDataSource()
    {
        var headerRegistration = UICollectionViewCellRegistration.GetRegistration(
            typeof(UICollectionViewListCell),
            new UICollectionViewCellRegistrationConfigurationHandler((cell, indexpath, item) =>
            {
                var sidebarItem = (SidebarItem)item;
                var contentConfiguration = UIListContentConfiguration.SidebarHeaderConfiguration;
                ((UICollectionViewListCell)cell).Accessories = new UICellAccessory[]
                {
                    new UICellAccessoryOutlineDisclosure(),
                };
                contentConfiguration.Text = sidebarItem.Title;
                contentConfiguration.TextProperties.Font = UIFont.PreferredSubheadline;
                contentConfiguration.TextProperties.Color = UIColor.SecondaryLabel;
                cell.ContentConfiguration = contentConfiguration;
                // cell.AddInteraction(new MenuInteraction(sidebarItem, this.RemoveSidebarItem,
                //     this.RemoveSidebarItemFromFolder));
            }));

        var rowRegistration = UICollectionViewCellRegistration.GetRegistration(
            typeof(UICollectionViewListCell),
            new UICollectionViewCellRegistrationConfigurationHandler((cell, indexpath, item) =>
            {
                var sidebarItem = item as SidebarItem;
                if (sidebarItem is null)
                {
                    return;
                }

                var cfg = UIListContentConfiguration.SidebarCellConfiguration;
                cfg.Text = sidebarItem.Title;
                // switch (sidebarItem.SidebarItemType)
                // {
                //     case SidebarItemType.FeedListItem:
                //     {
                //         cfg.Image = sidebarItem.Image;
                //         if (cfg.Image is not null)
                //         {
                //             cfg.ImageProperties.CornerRadius = 3;
                //             cfg.ImageToTextPadding = 5;
                //             cfg.ImageProperties.ReservedLayoutSize = new CGSize(30, 30);
                //         }
                //
                //         break;
                //     }
                //
                //     case SidebarItemType.SmartFilter:
                //         cfg.Image = sidebarItem.Image;
                //         break;
                // }

                cell.ContentConfiguration = cfg;
                // cell.AddInteraction(new MenuInteraction(sidebarItem, this.RemoveSidebarItem,
                //     this.RemoveSidebarItemFromFolder));
                // ((UICollectionViewListCell)cell).Accessories = new UICellAccessory[] { sidebarItem.UnreadCountView };
            }));

        if (this.collectionView is null)
        {
            throw new NullReferenceException(nameof(this.collectionView));
        }

        return new UICollectionViewDiffableDataSource<NSString, SidebarItem>(
            this.collectionView,
            new UICollectionViewDiffableDataSourceCellProvider((collectionView, indexPath, item) =>
            {
                var sidebarItem = item as SidebarItem;
                if (sidebarItem is null || collectionView is null)
                {
                    throw new Exception();
                }

                return sidebarItem.SidebarItemType switch
                {
                    SidebarItemType.Header => collectionView.DequeueConfiguredReusableCell(
                        headerRegistration,
                        indexPath,
                        item),
                    _ =>
                        collectionView.DequeueConfiguredReusableCell(rowRegistration, indexPath, item),
                };
            })
        );
    }
}

public class SidebarItem : NSObject
{
    public string? Title { get; set; }
    
    public SidebarItemType SidebarItemType { get; set; }
    
    public List<SidebarItem>? Items { get; set; }
}

public enum SidebarItemType
{
    Header,
    Item
}