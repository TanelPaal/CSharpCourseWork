using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL.DB;

public class AppDbContext : DbContext
{
    public DbSet<GameConfiguration> GameConfigurations { get; set; } = default!;
    public DbSet<GameState> GameStates { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}