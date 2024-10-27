using DA.UI.Grid;

namespace CatalystGui;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // create a new window instance based on the screen size
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        Window.RootViewController = new GridViewController();
        // make the window visible
        Window.MakeKeyAndVisible();

        return true;
    }
}

public class GridViewController : UIViewController
{
    public GridViewController()
    {
        var grid = CreateGrid();
        //grid.AddChild(CreateGrid(), 1, 1);

        View.AddSubview(grid); 
        grid.TranslatesAutoresizingMaskIntoConstraints = false;
        grid.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
        grid.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
        grid.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
        grid.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
    }

    private UIGrid CreateGrid()
    {
        var grid = new UIGrid()
        {
            ShowGridLines = true // Helpful for debugging
        };

        // Define columns
        grid.AddColumn(new GridDefinition(100, GridUnit.Pixel));
        grid.AddColumn(new GridDefinition(1, GridUnit.Star));
        grid.AddColumn(new GridDefinition(100, GridUnit.Pixel));

        // Define rows
        grid.AddRow(new GridDefinition(100, GridUnit.Pixel));
        grid.AddRow(new GridDefinition(1, GridUnit.Star));
        grid.AddRow(new GridDefinition(100, GridUnit.Pixel));

        var view1 = new UIView() { BackgroundColor = UIColor.Red };
        grid.AddChild(view1, 0, 0);
        var view2 = new UIView() { BackgroundColor = UIColor.Blue };
        grid.AddChild(view2, 0, 2);
        var view3 = new UIView() { BackgroundColor = UIColor.Green };
        grid.AddChild(view3, 2, 0);
        var view4 = new UIView() { BackgroundColor = UIColor.Yellow };
        grid.AddChild(view4, 2, 2);
        return grid;
    }
}

public sealed class NestedGridViewController : UIViewController
{
    private UIGrid loginControlsGrid;
    private UIGrid imageGrid;
    private UIGrid layoutGrid;
    private UIImageView imageLogo;

    public NestedGridViewController()
    {
        Title = "Nested Grids";
        this.layoutGrid = new UIGrid() { ShowGridLines = true };
        
        this.layoutGrid.AddColumn(new GridDefinition(1, GridUnit.Star));
        
        // Header
        this.layoutGrid.AddRow(new GridDefinition(50));
        // Image
        this.layoutGrid.AddRow(new GridDefinition(1, GridUnit.Star));
        // Login
        this.layoutGrid.AddRow(new GridDefinition(150, GridUnit.Pixel));

        this.imageGrid = new UIGrid();
        this.imageGrid.AddRow(new GridDefinition(1, GridUnit.Star));
        this.imageGrid.AddColumn(new GridDefinition(1, GridUnit.Star));
        this.imageGrid.AddColumn(new GridDefinition(100, GridUnit.Auto, 100, 500));
        this.imageGrid.AddColumn(new GridDefinition(1, GridUnit.Star));
        UIImage logo = UIImage.GetSystemImage("star")!;
        this.imageLogo = new UIImageView(logo);
        this.imageLogo.BackgroundColor = UIColor.SystemGreen;
        this.imageGrid.AddChild(this.imageLogo, 0, 1, verticalAlignment: GridAlignment.Center, horizontalAlignment: GridAlignment.Center);
        this.layoutGrid.AddChild(new UIView() { BackgroundColor = UIColor.Blue}, 1, 0);
        
        this.loginControlsGrid = new UIGrid() { BackgroundColor = UIColor.Orange};
        this.loginControlsGrid.AddColumn(new GridDefinition(1, GridUnit.Star));
        this.loginControlsGrid.AddRow(new GridDefinition(0, GridUnit.Auto));
        this.loginControlsGrid.AddRow(new GridDefinition(0, GridUnit.Auto));
        this.loginControlsGrid.AddRow(new GridDefinition(0, GridUnit.Auto));
        var usernameField = new UITextField();
        usernameField.Placeholder = "Username";
        this.loginControlsGrid.AddChild(usernameField, 0, 0, margin: new UIEdgeInsets(10, 10, 10, 10));
        var passwordField = new UITextField() { SecureTextEntry = true };
        passwordField.Placeholder = "Password";
        this.loginControlsGrid.AddChild(passwordField, 1, 0, margin: new UIEdgeInsets(10, 10, 10, 10));

        var loginButton = UIButton.FromType(UIButtonType.System);
        loginButton.SetTitle("Login", UIControlState.Normal);
        this.loginControlsGrid.AddChild(loginButton, 2, 0, margin: new UIEdgeInsets(10, 10, 10, 10));
        
        this.layoutGrid.AddChild(this.loginControlsGrid, 2, 0);
        this.View = this.layoutGrid;
    }
}

public class GridScrollViewController : UIViewController
{
    private UIScrollView scrollView;
    private UIGrid contentView;
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        SetupScrollView();
        AddSampleContent();
    }
    
    private void SetupScrollView()
    {
        // Initialize ScrollView
        scrollView = new UIScrollView();
        scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
        View.AddSubview(scrollView);

        // Setup ScrollView constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            scrollView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            scrollView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
            scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
            scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor)
        });

        // Initialize content view
        contentView = new UIGrid();
        contentView.TranslatesAutoresizingMaskIntoConstraints = false;
        scrollView.AddSubview(contentView);

        // Setup content view constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            contentView.LeadingAnchor.ConstraintEqualTo(scrollView.LeadingAnchor),
            contentView.TrailingAnchor.ConstraintEqualTo(scrollView.TrailingAnchor),
            contentView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor),
            contentView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor),
            // This ensures the content view is the same width as the scroll view
            contentView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor)
        });
    }

    private void AddSampleContent()
    {
        var rowCount = 40;
        var random = new Random();
        contentView.AddColumn(new GridDefinition(1, GridUnit.Star));
        foreach (var row in Enumerable.Range(0, rowCount))
        {
            contentView.AddRow(new GridDefinition(100));
            var randomColor = UIColor.FromRGB(random.Next(256), random.Next(256), random.Next(256));
            contentView.AddChild(new UIView() { BackgroundColor = randomColor }, row, 0);
        }
        
        contentView.LayoutIfNeeded();
        scrollView.ContentSize = new CGSize(scrollView.Frame.Width, contentView.Frame.Height);
    }
}

public class ScrollViewController : UIViewController
{
    private UIScrollView scrollView;
    private UIView contentView;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        View.BackgroundColor = UIColor.White;

        SetupScrollView();
        AddSampleContent();
    }

    private void SetupScrollView()
    {
        // Initialize ScrollView
        scrollView = new UIScrollView();
        scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
        View.AddSubview(scrollView);

        // Setup ScrollView constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            scrollView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            scrollView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
            scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
            scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor)
        });

        // Initialize content view
        contentView = new UIView();
        contentView.TranslatesAutoresizingMaskIntoConstraints = false;
        scrollView.AddSubview(contentView);

        // Setup content view constraints
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            contentView.LeadingAnchor.ConstraintEqualTo(scrollView.LeadingAnchor),
            contentView.TrailingAnchor.ConstraintEqualTo(scrollView.TrailingAnchor),
            contentView.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor),
            contentView.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor),
            // This ensures the content view is the same width as the scroll view
            contentView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor)
        });
    }

    private void AddSampleContent()
    {
        // Add a title label
        var titleLabel = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Text = "Scrollable Content Demo",
            Font = UIFont.BoldSystemFontOfSize(24),
            TextAlignment = UITextAlignment.Center
        };
        contentView.AddSubview(titleLabel);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            titleLabel.TopAnchor.ConstraintEqualTo(contentView.TopAnchor, 20),
            titleLabel.LeadingAnchor.ConstraintEqualTo(contentView.LeadingAnchor, 20),
            titleLabel.TrailingAnchor.ConstraintEqualTo(contentView.TrailingAnchor, -20)
        });

        // Create colored boxes
        var colors = new UIColor[]
        {
            UIColor.SystemRed,
            UIColor.SystemBlue,
            UIColor.SystemGreen,
            UIColor.SystemOrange,
            UIColor.SystemPurple
        };

        UIView previousBox = titleLabel;

        for (int i = 0; i < colors.Length; i++)
        {
            var box = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = colors[i]
            };

            var label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = $"Box {i + 1}",
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(20)
            };

            box.AddSubview(label);
            contentView.AddSubview(box);

            // Constrain the box
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                box.TopAnchor.ConstraintEqualTo(previousBox.BottomAnchor, 20),
                box.LeadingAnchor.ConstraintEqualTo(contentView.LeadingAnchor, 20),
                box.TrailingAnchor.ConstraintEqualTo(contentView.TrailingAnchor, -20),
                box.HeightAnchor.ConstraintEqualTo(200)
            });

            // Constrain the label within the box
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                label.CenterXAnchor.ConstraintEqualTo(box.CenterXAnchor),
                label.CenterYAnchor.ConstraintEqualTo(box.CenterYAnchor)
            });

            previousBox = box;
        }

        // Add bottom constraint to the last box
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            previousBox.BottomAnchor.ConstraintEqualTo(contentView.BottomAnchor, -20)
        });
    }
}