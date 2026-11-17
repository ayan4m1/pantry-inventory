namespace Inventory.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string Manufacturer { get; set; }
        public required string Name { get; set; }
        public float? Cost { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
