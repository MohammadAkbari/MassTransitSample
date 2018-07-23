using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Core
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=BlogDb;Integrated Security=true;");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();

            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            foreach (var entry in entries)
            {
                foreach (var prop in entry.Properties)
                {
                    if ((entry.State == EntityState.Added || prop.IsModified) && prop.OriginalValue is string)
                    {
                        if (string.IsNullOrEmpty(prop.CurrentValue.ToString()))
                        {
                            prop.CurrentValue = null;
                        }
                    }

                }
            }

            ChangeTracker.AutoDetectChangesEnabled = false;
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }
    }
}
