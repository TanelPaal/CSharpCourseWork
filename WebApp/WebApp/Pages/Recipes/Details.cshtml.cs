using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.Recipes;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(AppDbContext context, ILogger<DetailsModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Recipe Recipe { get; set; } = default!;
    
    [BindProperty]
    public Rating Rating { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Ratings)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (recipe == null) return NotFound();
        Recipe = recipe;
        Rating.RecipeId = recipe.Id;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation($"OnPostAsync called with Rating.RecipeId={Rating.RecipeId}, Rating.Score={Rating.Score}");

        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError($"Model validation error: {error.ErrorMessage}");
                }
            }
            
            // Load the recipe data for the page
            var currentRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(m => m.Id == Rating.RecipeId);

            if (currentRecipe == null) return NotFound();
            Recipe = currentRecipe;
            return Page();
        }

        try
        {
            _logger.LogInformation("Model is valid, proceeding with rating save");
            
            // Set creation timestamp
            Rating.CreatedAt = DateTime.UtcNow;

            // Debug info
            _logger.LogInformation($"Adding rating: RecipeId={Rating.RecipeId}, Score={Rating.Score}, Comment={Rating.Comment}, CreatedAt={Rating.CreatedAt}");

            // Add the new rating
            await _context.Ratings.AddAsync(Rating);
            var saveResult = await _context.SaveChangesAsync();
            
            _logger.LogInformation($"SaveChanges result: {saveResult} entities saved");

            // Reload the recipe with its ratings to calculate the new average
            var recipe = await _context.Recipes
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == Rating.RecipeId);

            if (recipe != null)
            {
                var oldAverage = recipe.AverageRating;
                recipe.AverageRating = recipe.Ratings.Average(r => (double)r.Score);
                _logger.LogInformation($"Updated average rating from {oldAverage} to {recipe.AverageRating}");
                
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = Rating.RecipeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving rating: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            ModelState.AddModelError("", "Error saving rating. Please try again.");
            
            // Reload the recipe data
            var currentRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(m => m.Id == Rating.RecipeId);

            if (currentRecipe == null) return NotFound();
            Recipe = currentRecipe;
            return Page();
        }
    }
} 