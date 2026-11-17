using Inventory.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(InventoryDbContext db) : ControllerBase
    {
        [HttpGet]
        [EndpointName("getProduct")]
        [ProducesResponseType<Product>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> Get([FromQuery] Guid productId)
        {
            var product = await db.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
