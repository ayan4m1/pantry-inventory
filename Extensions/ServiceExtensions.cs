using Inventory.Entities;
using Inventory.Responses;

namespace Inventory.Extensions
{
    public static class ServiceExtensions
    {
        public static ProductResponse ToProductResponse(this Product product) => new(
            product.Id,
            product.HouseholdId,
            product.Name,
            product.Unit,
            product.Quantity,
            product.LowStockThreshold,
            product.ReorderQuantity,
            product.Cost,
            product.Quantity < product.LowStockThreshold,
            product.Version,
            product.UpdatedAt
        );
    }
}
