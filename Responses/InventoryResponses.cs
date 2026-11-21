namespace Inventory.Responses
{
    public record ProductResponse(
        Guid Id,
        Guid HouseholdId,
        string Name,
        string? Unit,
        decimal Quantity,
        decimal? LowStockThreshold,
        decimal? ReorderQuantity,
        decimal? Cost,
        bool IsLowStock,
        uint Version,
        DateTimeOffset UpdatedAt
    );
}
