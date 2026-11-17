namespace Inventory.Entities
{
    public class ProductStock
    {
        public Guid Id { get; set; }
        public required Product Product { get; set; }
        public required string HouseholdId { get; set; }
        public required int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
