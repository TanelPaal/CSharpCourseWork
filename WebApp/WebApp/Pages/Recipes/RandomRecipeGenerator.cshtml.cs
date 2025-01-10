using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Recipes;

public class RandomRecipeGeneratorModel : PageModel
{
    private readonly AppDbContext _context;
    private static readonly string[] RecipeAdjectives = {
        "Crazy", "Wild", "Unexpected", "Bizarre", "Quirky",
        "Wacky", "Outrageous", "Peculiar", "Funky", "Eccentric"
    };

    private static readonly string[] RecipeFormats = {
        "{0} {1} Surprise",
        "{0} {1} Fusion",
        "{0} {1} Adventure",
        "{0} {1} Creation",
        "The {0} {1} Experience"
    };

    private static readonly string[] CookingMethods = {
        "stir-fry", "bake", "blend", "mix", "toss",
        "combine", "layer", "sprinkle", "drizzle"
    };

    public RandomRecipeGeneratorModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Recipe? GeneratedRecipe { get; set; }

    [BindProperty]
    public List<int> IngredientIds { get; set; } = new();

    [BindProperty]
    public List<string> Quantities { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        GeneratedRecipe = await GenerateRandomRecipe();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (GeneratedRecipe == null) return RedirectToPage();

        var recipe = new Recipe
        {
            Title = GeneratedRecipe.Title,
            Description = GeneratedRecipe.Description,
            Instructions = GeneratedRecipe.Instructions,
            PrepTimeMinutes = GeneratedRecipe.PrepTimeMinutes,
            ImageUrl = GeneratedRecipe.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient>()
        };

        // Add recipe ingredients
        for (int i = 0; i < IngredientIds.Count; i++)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = IngredientIds[i],
                Quantity = Quantities[i]
            });
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Recipes/Details", new { id = recipe.Id });
    }

    private async Task<Recipe> GenerateRandomRecipe()
    {
        var random = new Random();
        var ingredients = await _context.Ingredients.ToListAsync();
        
        if (!ingredients.Any())
            throw new InvalidOperationException("No ingredients available in the database");

        var selectedIngredients = ingredients
            .OrderBy(x => random.Next())
            .Take(3)
            .ToList();

        var quantities = new List<string>
        {
            $"{random.Next(1, 5)} cups",
            $"{random.Next(1, 6)} tablespoons",
            $"{random.Next(1, 4)} pieces"
        };

        var recipe = new Recipe
        {
            Title = GenerateRandomTitle(selectedIngredients[0].Name),
            Description = $"A daring combination of {selectedIngredients[0].Name}, {selectedIngredients[1].Name}, and {selectedIngredients[2].Name} that will surprise your taste buds!",
            PrepTimeMinutes = random.Next(5, 60),
            Instructions = GenerateRandomInstructions(
                selectedIngredients.Select(i => i.Name).ToList(),
                quantities
            ),
            ImageUrl = $"https://picsum.photos/seed/{Guid.NewGuid()}/400/300",
            CreatedAt = DateTime.UtcNow,
            RecipeIngredients = new List<RecipeIngredient>()
        };

        for (int i = 0; i < selectedIngredients.Count; i++)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = selectedIngredients[i].Id,
                Ingredient = selectedIngredients[i],
                Quantity = quantities[i]
            });
        }

        return recipe;
    }

    private string GenerateRandomTitle(string mainIngredient)
    {
        var random = new Random();
        var adjective = RecipeAdjectives[random.Next(RecipeAdjectives.Length)];
        var format = RecipeFormats[random.Next(RecipeFormats.Length)];
        return string.Format(format, adjective, mainIngredient);
    }

    private string GenerateRandomInstructions(List<string> ingredients, List<string> quantities)
    {
        var random = new Random();
        var instructions = new List<string>();
        var cookingMethod = CookingMethods[random.Next(CookingMethods.Length)];
        
        instructions.Add($"Gather all ingredients: {string.Join(", ", ingredients)}");
        instructions.Add($"In a large bowl, {cookingMethod} {quantities[0]} of {ingredients[0]} with {quantities[1]} of {ingredients[1]}");
        instructions.Add($"Add {quantities[2]} of {ingredients[2]} and mix well");
        instructions.Add("Let the mixture rest for 5 minutes");
        instructions.Add($"Serve your creation and enjoy the unexpected flavors!");
        
        return string.Join("\n", instructions);
    }
}
