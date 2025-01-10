namespace Domain;

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public string Quantity { get; set; } = default!;
    
    // Navigation properties
    public Recipe Recipe { get; set; } = default!;
    public Ingredient Ingredient { get; set; } = default!;
}