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
        [HttpDelete("household/{householdId:guid}/stock")]
        public async Task<IActionResult> DeleteHouseholdStock(Guid householdId)
        {
            await db.ProductStocks
                .Where((stock) => stock.HouseholdId == householdId)
                .ExecuteDeleteAsync();

            return NoContent();
        }
    }
}
