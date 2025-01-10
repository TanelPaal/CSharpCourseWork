using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Rating
{
    public int Id { get; set; }
    
    [Required]
    public int RecipeId { get; set; }
    
    [Required]
    [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars")]
    public int Score { get; set; }
    
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    [ForeignKey("RecipeId")]
    public virtual Recipe? Recipe { get; set; }  // Make nullable and virtual
}