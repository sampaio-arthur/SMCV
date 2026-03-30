using Microsoft.EntityFrameworkCore;
using SMCV.Entities;

namespace SMCV.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add DbSet<YourEntity> for each entity in your domain.
    public DbSet<Example> Examples { get; set; }
}
