using System.Security.Claims;
using FlashCart.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly FlashCartDbContext _context;

    public OrdersController(FlashCartDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userIdValue = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var orders = await _context.Orders
            .Where(order => order.UserId == userId)
            .Include(order => order.OrderItems)
            .ToListAsync();

        return Ok(orders);
    }
}