using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Recipe> Recipes { get; set; } = default!;
    public Recipe? RandomRecipe { get; set; }

    public async Task OnGetAsync()
    {
        Recipes = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostSpinRouletteAsync()
    {
        var count = await _context.Recipes.CountAsync();
        if (count == 0) return RedirectToPage();

        var random = new Random();
        var skip = random.Next(count);
        
        RandomRecipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Skip(skip)
            .FirstOrDefaultAsync();

        return RedirectToPage(new { recipeId = RandomRecipe?.Id });
    }
}