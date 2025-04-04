using kairos_api.Models;
using Microsoft.EntityFrameworkCore;

namespace kairos_api;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Timecapsule> Timecapsules { get; set; }
}
