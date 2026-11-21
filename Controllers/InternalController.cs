using Inventory.Entities;
using Inventory.Requests;
using Inventory.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    // Service-to-service endpoints; the gateway blocks /api/{service}/internal/** from the outside.
    [ApiController]
    [Route("internal")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InternalController(InventoryDbContext db) : ControllerBase
    {
        [HttpDelete("products/{id:guid}/restock")]
        [ProducesResponseType<Product>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> RestockProduct(Guid id, [FromBody] RestockRequest request)
        {
            var product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var restock = await db.ProcessedRestocks.Where(pr => pr.ProductId == id && pr.ShoppingItemId == request.ShoppingItemId).FirstAsync();

            if (restock == null)
            {

                restock = new ProcessedRestock()
                {
                    ProductId = id,
                    ShoppingItemId = request.ShoppingItemId,
                    Quantity = request.Quantity,
                    AppliedAt = DateTime.Now
                };

                await db.ProcessedRestocks.AddAsync(restock);
            } else
            {
                restock.Quantity += request.Quantity;
            }

            await db.SaveChangesAsync();

            return Ok(new ProductResponse(
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
            ));
        }
    }
}
