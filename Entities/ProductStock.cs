namespace Inventory.Entities
{
    public class ProductStock
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid HouseholdId { get; set; }
        public required int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Product Product { get; set; } = null!;
    }
}
