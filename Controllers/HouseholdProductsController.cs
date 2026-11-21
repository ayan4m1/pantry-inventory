using Inventory.Entities;
using Inventory.Requests;
using Inventory.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("households/{householdId:guid}")]
    public class HouseholdProductsController(InventoryDbContext db) : ControllerBase
    {
        [HttpGet("products")]
        [EndpointName("listProducts")]
        [ProducesResponseType<List<Product>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> List(Guid householdId, [FromQuery] bool lowStock, [FromQuery] string search)
        {
            return Ok(await db.Products
                .Where(p =>
                    p.Name.Contains(search) &&
                    p.HouseholdId == householdId &&
                    (p.Quantity < p.LowStockThreshold) == lowStock
                ).Select((p) => new ProductResponse(
                    p.Id,
                    p.HouseholdId,
                    p.Name,
                    p.Unit,
                    p.Quantity,
                    p.LowStockThreshold,
                    p.ReorderQuantity,
                    p.Cost,
                    p.Quantity < p.LowStockThreshold,
                    p.Version,
                    p.UpdatedAt
                )
            ).ToListAsync());
        }

        [HttpGet("products/{id:guid}")]
        [EndpointName("getProduct")]
        [ProducesResponseType<Product>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> Get(Guid householdId, Guid id)
        {
            var product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.HouseholdId != householdId)
            {
                return NotFound();
            }

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

        [HttpPost("products")]
        [EndpointName("createProduct")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductResponse>> Create(Guid householdId, [FromBody] CreateProductRequest request)
        {
            var product = new Product()
            {
                Cost = request.Cost,
                CreatedAt = DateTime.Now,
                HouseholdId = householdId,
                Name = request.Name,
                LowStockThreshold = request.LowStockThreshold,
                Quantity = request.Quantity,
                ReorderQuantity = request.ReorderQuantity,
                Unit = request.Unit,
                UpdatedAt = DateTime.Now,
                Version = 1
            };

            var result = await db.Products.AddAsync(product);

            await db.SaveChangesAsync();

            return Ok(new ProductResponse(
                result.Entity.Id,
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

        [HttpPut("products/{id:guid}")]
        [EndpointName("updateProduct")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> Update(Guid householdId, Guid id, [FromBody] UpdateProductRequest request)
        {
            var product = await db.Products.FindAsync(id);

            if (product == null || product.HouseholdId != householdId)
            {
                return NotFound();
            }

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

        [HttpPost("products/{id:guid}/adjustments")]
        [EndpointName("adjustProductQuantity")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> AdjustQuantity(Guid householdId, Guid id, [FromBody] AdjustQuantityRequest request)
        {
            var product = await db.Products.FindAsync(id);

            if (product == null || product.HouseholdId != householdId)
            {
                return NotFound();
            }

            product.Quantity += request.Delta;

            if (product.Quantity < 0)
            {
                return BadRequest();
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

        [HttpDelete("products/{id:guid}")]
        [EndpointName("deleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid householdId, Guid id)
        {
            var query = db.Products.Where(p => p.HouseholdId == householdId && p.Id == id);

            if (await query.FirstAsync() == null)
            {
                return NotFound();
            }
            
            await query.ExecuteDeleteAsync();

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
