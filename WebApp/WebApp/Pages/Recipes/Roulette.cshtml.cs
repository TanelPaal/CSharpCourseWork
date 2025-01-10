using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Recipes;

public class RouletteModel : PageModel
{
    private readonly AppDbContext _context;

    public RouletteModel(AppDbContext context)
    {
        _context = context;
    }

    public Recipe? RandomRecipe { get; set; }
    public bool NoRecipesFound { get; set; }

    public IActionResult OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var count = await _context.Recipes.CountAsync();
        if (count == 0)
        {
            NoRecipesFound = true;
            return Page();
        }

        var random = new Random();
        var skip = random.Next(count);

        RandomRecipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Skip(skip)
            .FirstOrDefaultAsync();

        return Page();
    }
} 