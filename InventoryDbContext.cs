using Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductStock> ProductStocks => Set<ProductStock>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.Property(h => h.Name).HasMaxLength(100);
            });

            b.Entity<ProductStock>(e =>
            {
                e.HasKey(m => new { m.HouseholdId, m.ProductId });
                e.HasIndex(m => m.HouseholdId);
                e.Property(h => h.HouseholdId).HasMaxLength(64);
            });
        }
    }
}
