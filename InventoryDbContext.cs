using Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProcessedRestock> ProcessedRestocks => Set<ProcessedRestock>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).HasMaxLength(100).IsRequired();
                e.Property(p => p.Unit).HasMaxLength(20);

                e.Property(p => p.Quantity).HasPrecision(10, 2);
                e.Property(p => p.LowStockThreshold).HasPrecision(10, 2);
                e.Property(p => p.ReorderQuantity).HasPrecision(10, 2);
                e.Property(p => p.Cost).HasPrecision(10, 2);

                e.Property(p => p.Version).IsRowVersion();       // same as [Timestamp]: Npgsql maps a uint row version to xmin

                e.HasIndex(p => new { p.HouseholdId, p.Name }).IsUnique();

                e.ToTable(t =>
                {
                    t.HasCheckConstraint("ck_products_quantity_non_negative", "quantity >= 0");
                    t.HasCheckConstraint("ck_products_cost_non_negative", "cost >= 0");   // NULL passes a check constraint
                });
            });

            b.Entity<ProcessedRestock>(e =>
            {
                e.HasKey(r => r.ShoppingItemId);
                e.Property(r => r.ShoppingItemId).ValueGeneratedNever();   // the id comes from Shopping
                e.Property(r => r.Quantity).HasPrecision(10, 2);

                e.HasOne<Product>()
                 .WithMany()
                 .HasForeignKey(r => r.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);              // same database, so a real FK is fine here
            });
        }
    }
}
