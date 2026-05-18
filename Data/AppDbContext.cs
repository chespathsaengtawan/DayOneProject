using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Add your DbSet properties here, e.g.:
    // public DbSet<YourEntity> YourEntities => Set<YourEntity>();
}
