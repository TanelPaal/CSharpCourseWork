using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Recipes;

public class BrowseModel : PageModel
{
    private readonly AppDbContext _context;

    public BrowseModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Recipe> Recipes { get; set; } = default!;
    public string? PrepTimeRange { get; set; }
    public int? MinRating { get; set; }

    public async Task OnGetAsync(string? prepTimeRange, int? minRating)
    {
        PrepTimeRange = prepTimeRange;
        MinRating = minRating;

        var query = _context.Recipes
            .Include(r => r.Ratings)
            .AsQueryable();

        if (!string.IsNullOrEmpty(prepTimeRange))
        {
            switch (prepTimeRange)
            {
                case "0-15":
                    query = query.Where(r => r.PrepTimeMinutes <= 15);
                    break;
                case "15-30":
                    query = query.Where(r => r.PrepTimeMinutes > 15 && r.PrepTimeMinutes <= 30);
                    break;
                case "30-60":
                    query = query.Where(r => r.PrepTimeMinutes > 30 && r.PrepTimeMinutes <= 60);
                    break;
                case "60+":
                    query = query.Where(r => r.PrepTimeMinutes > 60);
                    break;
            }
        }

        if (minRating.HasValue)
        {
            query = query.Where(r => r.AverageRating >= minRating.Value);
        }

        Recipes = await query.ToListAsync();
    }
} 