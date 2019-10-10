using Microsoft.EntityFrameworkCore;
using Migrator.Entities;

namespace Migrator
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FooTable>();
        }
    }
}
