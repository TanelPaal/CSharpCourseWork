using Microsoft.EntityFrameworkCore;

namespace DAL.DB;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}