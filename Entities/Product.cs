using System.ComponentModel.DataAnnotations;

namespace Inventory.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid HouseholdId { get; set; }
        public required string Name { get; set; }        // max 100, unique per household
        public string? Unit { get; set; }                 // "can", "lb", "box"; max 20
        public decimal Quantity { get; set; }             // numeric(10,2), check >= 0
        public decimal? Cost { get; set; }                 // price per Unit, numeric(10,2), check >= 0; null = unknown
        public decimal? LowStockThreshold { get; set; }   // null = never auto-add to the list
        public decimal? ReorderQuantity { get; set; }     // amount to put on the list; null = 1
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        [Timestamp]
        public uint Version { get; set; }                 // maps to Postgres xmin: optimistic concurrency

        public List<ProcessedRestock> ProcessedRestocks { get; set; } = null!;
    }
}
