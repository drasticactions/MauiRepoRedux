using RecipeFinder.Models;
using RecipeFinder.Services;

namespace RecipeFinder.Views;

public partial class RecipeListPage : ContentPage
{
	private readonly RecipeService _recipeService;
	private List<Recipe> _allRecipes;
	private List<string> _categories;

	public RecipeListPage()
	{
		InitializeComponent();
		_recipeService = new RecipeService();

		// Get initial data
		_allRecipes = _recipeService.GetAllRecipes();
		_categories = _recipeService.GetCategories();

		// Populate picker
		CategoryPicker.Items.Clear();
		CategoryPicker.Items.Add("All");
		foreach (var category in _categories)
		{
			CategoryPicker.Items.Add(category);
		}

		// Set default selection
		CategoryPicker.SelectedIndex = 0;

		// Display recipes
		RecipesCollection.ItemsSource = _allRecipes;
	}

	private void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (CategoryPicker.SelectedIndex == 0)
		{
			// "All" is selected
			RecipesCollection.ItemsSource = _allRecipes;
		}
		else
		{
			var selectedCategory = CategoryPicker.SelectedItem.ToString();
			RecipesCollection.ItemsSource = _recipeService.GetRecipesByCategory(selectedCategory);
		}
	}
	private async void RecipesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
		{
			var queryParamters = new ShellNavigationQueryParameters
			{
				{ "Recipe", selectedRecipe },
				{ "RecipeService", _recipeService }
			};

			await Shell.Current.GoToAsync(nameof(RecipeDetailPage), queryParamters);

			// Clear selection
			RecipesCollection.SelectedItem = null;
		}
	}

	private void FavoriteTapped(object sender, TappedEventArgs e)
	{
		if (e.Parameter is string recipeId)
		{
			_recipeService.ToggleFavorite(recipeId);

			// Refresh list to show updated favorite status
			RecipesCollection.ItemsSource = null;
			if (CategoryPicker.SelectedIndex == 0)
			{
				RecipesCollection.ItemsSource = _allRecipes;
			}
			else
			{
				var selectedCategory = CategoryPicker.SelectedItem.ToString();
				RecipesCollection.ItemsSource = _recipeService.GetRecipesByCategory(selectedCategory);
			}
		}
	}
}