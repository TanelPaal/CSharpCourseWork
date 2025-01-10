using Microsoft.EntityFrameworkCore;
using Domain;

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Recipe> Recipes { get; set; } = default!;
    public DbSet<Ingredient> Ingredients { get; set; } = default!;
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = default!;
    public DbSet<Rating> Ratings { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeIngredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Ratings)
            .WithOne(rating => rating.Recipe)
            .HasForeignKey(rating => rating.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Ingredient>()
            .HasMany(i => i.RecipeIngredients)
            .WithOne(ri => ri.Ingredient)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 