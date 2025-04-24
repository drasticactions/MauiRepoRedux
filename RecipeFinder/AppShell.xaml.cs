using RecipeFinder.Views;

namespace RecipeFinder;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
    }
}
