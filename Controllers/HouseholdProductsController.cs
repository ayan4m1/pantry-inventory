using Inventory.Entities;
using Inventory.Extensions;
using Inventory.Requests;
using Inventory.Responses;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("households/{householdId:guid}")]
    [Authorize(Policy = Policies.HouseholdMember)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class HouseholdProductsController(InventoryDbContext db) : ControllerBase
    {
        [HttpGet("products")]
        [EndpointName("listProducts")]
        [ProducesResponseType<List<ProductResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> List(
            Guid householdId,
            [FromQuery] bool? lowStock,
            [FromQuery] string? search,
            CancellationToken ct
        ) => Ok(await db.Products
            .Where(p =>
                (!string.IsNullOrWhiteSpace(search) && p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)) &&
                p.HouseholdId == householdId &&
                (!(lowStock ?? false) || (p.Quantity < p.LowStockThreshold))
            )
            .Select((p) => p.ToProductResponse())
            .ToListAsync(ct)
        );

        [HttpGet("products/{id:guid}")]
        [EndpointName("getProduct")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> Get(Guid householdId, Guid id, CancellationToken ct)
        {
            var product = await db.Products.FindAsync([id], ct);

            if (product == null)
            {
                return NotFound();
            }

            if (product.HouseholdId != householdId)
            {
                return NotFound();
            }

            return Ok(product.ToProductResponse());
        }

        [HttpPost("products")]
        [EndpointName("createProduct")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductResponse>> Create(Guid householdId, [FromBody] CreateProductRequest request, CancellationToken ct)
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

            await db.Products.AddAsync(product, ct);
            await db.SaveChangesAsync(ct);

            return Ok(product.ToProductResponse());
        }

        [HttpPut("products/{id:guid}")]
        [EndpointName("updateProduct")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> Update(Guid householdId, Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
        {
            var product = await db.Products.FindAsync([id], ct);

            if (product == null || product.HouseholdId != householdId)
            {
                return NotFound();
            }

            product.Cost = request.Cost;
            product.LowStockThreshold = request.LowStockThreshold;
            product.Name = request.Name;
            product.ReorderQuantity = request.ReorderQuantity;
            product.Unit = request.Unit;
            product.UpdatedAt = DateTime.UtcNow;
            product.Version++;

            return Ok(product.ToProductResponse());
        }

        [HttpPost("products/{id:guid}/adjustments")]
        [EndpointName("adjustProductQuantity")]
        [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> AdjustQuantity(Guid householdId, Guid id, [FromBody] AdjustQuantityRequest request, CancellationToken ct)
        {
            var product = await db.Products.FindAsync([id], ct);

            if (product == null || product.HouseholdId != householdId)
            {
                return NotFound();
            }

            product.Quantity += request.Delta;

            if (product.Quantity < 0)
            {
                return BadRequest();
            }

            await db.SaveChangesAsync(ct);

            return Ok(product.ToProductResponse());
        }

        [HttpDelete("products/{id:guid}")]
        [EndpointName("deleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid householdId, Guid id, CancellationToken ct)
        {
            var query = db.Products.Where(p => p.HouseholdId == householdId && p.Id == id);

            if (await query.FirstAsync(ct) == null)
            {
                return NotFound();
            }
            
            await query.ExecuteDeleteAsync(ct);

            await db.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}
