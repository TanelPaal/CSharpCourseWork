using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Recipes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Recipe Recipe { get; set; } = default!;
        
        public List<Ingredient> AvailableIngredients { get; set; } = default!;
        public List<Ingredient> Ingredients { get; set; } = new();
        
        [BindProperty]
        public List<int> SelectedIngredientIds { get; set; } = new();
        
        [BindProperty]
        public List<string> Quantities { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            AvailableIngredients = await _context.Ingredients.ToListAsync();
            // Start with 3 empty ingredient slots
            for (int i = 0; i < 3; i++)
            {
                Ingredients.Add(new Ingredient());
                SelectedIngredientIds.Add(0);
                Quantities.Add("");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableIngredients = await _context.Ingredients.ToListAsync();
                return Page();
            }

            Recipe.CreatedAt = DateTime.UtcNow;
            _context.Recipes.Add(Recipe);

            // Add recipe ingredients
            for (int i = 0; i < SelectedIngredientIds.Count; i++)
            {
                if (SelectedIngredientIds[i] != 0)
                {
                    Recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        IngredientId = SelectedIngredientIds[i],
                        Quantity = Quantities[i]
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
} 