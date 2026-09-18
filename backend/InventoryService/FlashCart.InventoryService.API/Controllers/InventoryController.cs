using FlashCart.InventoryService.Application.Common.Interfaces;
using FlashCart.InventoryService.Application.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryDbContext _dbContext;

    public InventoryController(
        IInventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetInventory(
        int productId)
    {
        var inventory = await _dbContext.Inventories
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.ProductId == productId);

        if (inventory == null)
            return NotFound(new
            {
                message = "Inventory not found."
            });

        var response = new InventoryResponse
        {
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity,
            ReservedQuantity = inventory.ReservedQuantity,
            AvailableQuantity =
                inventory.Quantity -
                inventory.ReservedQuantity
        };

        return Ok(response);
    }
}