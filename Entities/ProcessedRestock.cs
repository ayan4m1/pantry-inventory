namespace Inventory.Entities
{
    public class ProcessedRestock
    {
        public Guid ShoppingItemId { get; set; }          // PK: the idempotency key sent by Shopping
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public DateTimeOffset AppliedAt { get; set; }

        public Product Product { get; set; } = null!;
    }
}
