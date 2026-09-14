using System.Security.Claims;
using FlashCart.Application.DTO.Checkout;
using FlashCart.Domain.Entities;
using FlashCart.Domain.Enums;
using FlashCart.Domain.Services;
using FlashCart.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly FlashCartDbContext _dbContext;
    private readonly CartPricingService _pricingService;

    public CheckoutController(
        FlashCartDbContext dbContext,
        CartPricingService pricingService)
    {
        _dbContext = dbContext;
        _pricingService = pricingService;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .SingleOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                await transaction.RollbackAsync();

                return BadRequest(new
                {
                    message = "Cart not found."
                });
            }

            if (!cart.CartItems.Any())
            {
                await transaction.RollbackAsync();

                return BadRequest(new
                {
                    message = "Cart is empty."
                });
            }

            decimal subtotal = 0;

            foreach (var cartItem in cart.CartItems)
            {
                subtotal += _pricingService.CalculateSubtotal(
                    cartItem.Product.Price,
                    cartItem.Quantity);
            }

            var tax =
                _pricingService.CalculateTax(subtotal);

            var total =
                _pricingService.CalculateTotal(
                    subtotal,
                    tax);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Total = total,
            };

            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };

                order.OrderItems.Add(orderItem);
            }

            _dbContext.Orders.Add(order);

            _dbContext.CartItems.RemoveRange(
                cart.CartItems);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new CheckoutResponseDto
            {
                OrderId = order.Id,
                Total = order.Total,
                Status = order.Status.ToString()
            };

            return Ok(response);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}