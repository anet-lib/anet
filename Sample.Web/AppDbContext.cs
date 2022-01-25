using Microsoft.EntityFrameworkCore;
using Sample.Web.Models.Entities;

namespace Sample.Web;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AnetUser>();
    }
}
