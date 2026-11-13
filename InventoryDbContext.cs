using Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
