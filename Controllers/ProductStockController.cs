using Inventory.Entities;
using Inventory.Requests;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductStockController(InventoryDbContext db, IIdentityClient identityClient) : ControllerBase
    {
        [HttpPut("create")]
        [EndpointName("createProductStock")]
        [ProducesResponseType<ProductStock>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ProductStock>> Create([FromBody] ProductStockCreateRequest request)
        {
            var userId = User.FindFirstValue("sub");

            if (userId == null)
            {
                return Unauthorized();
            }

            // Households live in the identity service, so verify existence + membership there
            // (fail closed if identity is unreachable).
            try
            {
                if (!await identityClient.IsHouseholdMemberAsync(request.HouseholdId, userId))
                {
                    return NotFound("Household not found!");
                }
            }
            catch (HttpRequestException)
            {
                return Problem("Unable to verify household membership.", statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            if (!await db.Products.AnyAsync((product) => product.Id == request.ProductId))
            {
                return BadRequest("Invalid product specified!");
            }

            if (await db.ProductStocks.AnyAsync((stock) => stock.HouseholdId == request.HouseholdId && stock.ProductId == request.ProductId))
            {
                return Conflict("Stock for this product already exists in the household!");
            }

            var stock = new ProductStock()
            {
                HouseholdId = request.HouseholdId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await db.ProductStocks.AddAsync(stock);

            await db.SaveChangesAsync();

            return Created((string?)null, stock);
        }
    }
}
