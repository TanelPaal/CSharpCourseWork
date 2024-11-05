using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL.DB;

public class AppDbContext : DbContext
{
    public DbSet<Configuration> GameConfigurations { get; set; } = default!;
    public DbSet<SavedGame> SavedGames { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}