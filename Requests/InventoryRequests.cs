namespace Inventory.Requests
{
    public record CreateProductRequest(string Name, string? Unit, decimal Quantity, decimal? LowStockThreshold, decimal? ReorderQuantity, decimal? Cost);
    public record UpdateProductRequest(string Name, string? Unit, decimal? LowStockThreshold, decimal? ReorderQuantity, decimal? Cost, uint Version);
    public record AdjustQuantityRequest(decimal Delta);            // -1 = used one, +3 = bought three
    public record RestockRequest(Guid ShoppingItemId, decimal Quantity);
}
