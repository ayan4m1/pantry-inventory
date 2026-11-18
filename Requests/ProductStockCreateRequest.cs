namespace Inventory.Requests
{
    public record ProductStockCreateRequest(Guid HouseholdId, Guid ProductId, int Quantity);
}
