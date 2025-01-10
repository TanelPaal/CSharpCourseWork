using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Recipes.TopRated;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public List<Recipe> TopRecipes { get; set; } = new();

    public async Task OnGetAsync()
    {
        TopRecipes = await _context.Recipes
            .Include(r => r.Ratings)
            .Where(r => r.Ratings != null && r.Ratings.Any())
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.Ratings.Count)
            .Take(6)
            .ToListAsync();

        _logger.LogInformation($"Retrieved {TopRecipes.Count} top-rated recipes");
    }
} 