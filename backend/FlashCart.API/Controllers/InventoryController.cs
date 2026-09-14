using FlashCart.Application.DTO.Inventory;
using FlashCart.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    
    private readonly FlashCartDbContext _dbContext;

    public InventoryController(FlashCartDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveStock(ReserveStockRequestDto request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var inventory = await _dbContext.Inventories.FromSqlInterpolated(
                $"""
                SELECT * from "Inventories"
                WHERE "ProductId" = {request.ProductId}
                FOR UPDATE
                """
            ).SingleOrDefaultAsync();

              if (inventory == null)
            {
                await transaction.RollbackAsync();

                return NotFound(new
                {
                    message = "Inventory not found."
                });
            }

             var availableQuantity = inventory.Quantity - inventory.ReservedQuantity;

               if (availableQuantity < request.Quantity)
            {
                await transaction.RollbackAsync();

                return Conflict(new
                {
                    message = "Insufficient stock.",
                    availableQuantity
                });
            }

              inventory.ReservedQuantity +=
                request.Quantity;

            inventory.UpdatedAt =
                DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
             var response =
                new ReserveStockResponseDto
                {
                    Message =
                        "Stock reserved successfully.",

                    ProductId =
                        inventory.ProductId,

                    QuantityReserved =
                        request.Quantity,

                    RemainingQuantity =
                        inventory.Quantity
                        - inventory.ReservedQuantity
                };

            return Ok(response);
        }
        catch(Exception ex)
        {
            Console.WriteLine("=================================");
    Console.WriteLine("RESERVATION EXCEPTION");
    Console.WriteLine(ex.ToString());
    Console.WriteLine("=================================");

    await transaction.RollbackAsync();

    throw;
        }
    }
}