using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Recipe
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = default!;
    
    [Required]
    public string Description { get; set; } = default!;
    
    public string? ImageUrl { get; set; }
    
    [Required]
    [Range(1, 1440)]
    public int PrepTimeMinutes { get; set; }
    
    [Required]
    public string Instructions { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public double AverageRating { get; set; }
    
    // Navigation properties
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}