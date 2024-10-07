namespace TabBarTest;

public sealed class MainViewController : UITabBarController
{
    public MainViewController()
    {
        // You can either set tabs with UITabs, or use the View Controllers, but not both.
        // Setting UITabs overrides ViewControllers.
        
        this.TabGroupSetup();
        
        //this.SetTabs();
        
        // this.SetViewControllers();

        this.Mode = UITabBarControllerMode.TabSidebar;
        this.Sidebar.PreferredLayout = UITabBarControllerSidebarLayout.Tile;
        
        var test = UIListContentConfiguration.HeaderConfiguration;
        test.Text = "Header Configuration";
        test.Image = UIImage.GetSystemImage("star.fill");
        test.SecondaryText = "Header Secondary Text";

        var bottomTest = UIListContentConfiguration.FooterConfiguration;
        bottomTest.Text = "Footer Configuration";
        bottomTest.Image = UIImage.GetSystemImage("star.fill");
        bottomTest.SecondaryText = "Footer Secondary Text";
        this.Sidebar.HeaderContentConfiguration = test;
        this.Sidebar.FooterContentConfiguration = bottomTest;
        this.Sidebar.BottomBarView = new UIView(new CGRect(0, 0, 100, 400))
        {
            BackgroundColor =  UIColor.Red
        };
    }

    private void TabGroupSetup()
    {
        var testTab = new UITab[]
        {
            new UITab("Test 1", UIImage.GetSystemImage("star.fill"), "4", (tab => new TestViewController("Test 1"))),
            new UITab("Test 3", UIImage.GetSystemImage("star.fill"), "6", (tab => new TestViewController("Test 3")))
        };
        
        var testTab2 = new UITab[]
        {
            new UITab("Test 2", UIImage.GetSystemImage("star.fill"), "5", (tab => new TestViewController("Test 2")))
        };
        
        var tabGroups = new UITabGroup("Test", UIImage.GetSystemImage("star.fill"), "test2", testTab, (group
            =>
        {
            var test = new TestViewController("Section Group 3");
            return test;
        }));
        var tabGroups2 = new UITabGroup("Test2", UIImage.GetSystemImage("star.fill"), "test3", testTab2, (group => new TestViewController("Section Group 2")));
        
        this.Tabs = new UITab[]
        {
            new UITab("Fun Tab", UIImage.GetSystemImage("star.fill"), "FunTab", (tab => new TestViewController("Test"))),
            tabGroups2,
            tabGroups,
            new UITab("Funner Tab", UIImage.GetSystemImage("star.fill"), "FunnerTab", (tab => new TestViewController("Test"))),
        };
    }

    private void SetTabs()
    {
        var testTab = new UITab[]
        {
            new UITab("Test 1", UIImage.GetSystemImage("star.fill"), "50", (tab => new TestViewController("Test 1")))
        };
        
        this.Tabs = testTab;
    }

    private void SetViewControllers()
    {
        var tabs = new UIViewController[]
        {
            new TestViewController("Favorites") { TabBarItem = new UITabBarItem(UITabBarSystemItem.Favorites, 0) },
            new TestViewController("Bookmarks") { TabBarItem = new UITabBarItem(UITabBarSystemItem.Bookmarks, 1) },
            new TestViewController("History") { TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 2) },
        };
        
        this.ViewControllers = tabs;
        this.SelectedViewController = tabs[1];
    }
}

public sealed class TestViewController : UIViewController
{
    public TestViewController(string title)
    {
        var label = new UILabel(new CGRect(0, 0, 300, 40))
        {
            Text = title,
            TextAlignment = UITextAlignment.Center
        };
        this.View!.AddSubview(label);
        
        this.Title = title;
        
        label.AutoresizingMask = UIViewAutoresizing.All;
        label.Center = this.View!.Center;
    }
}