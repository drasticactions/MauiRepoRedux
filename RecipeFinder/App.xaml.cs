using RecipeFinder.Services;

namespace RecipeFinder;

public partial class App : Application
{
	private readonly RecipeService _recipeService;

	public App(RecipeService recipeService)
	{
		InitializeComponent();
		_recipeService = recipeService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}