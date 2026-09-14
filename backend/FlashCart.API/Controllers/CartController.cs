using System.Security.Claims;
using FlashCart.Application.DTO.Cart;
using FlashCart.Domain.Services;
using FlashCart.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly FlashCartDbContext _context;
    private readonly CartPricingService _cartPricingService;

    public CartController(FlashCartDbContext context, CartPricingService cartPricingService)
    {
        _context = context;
        _cartPricingService = cartPricingService;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart(
        AddToCartRequestDto request)
    {
        var userIdValue = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p =>
                p.Id == request.ProductId);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        var cart = await _context.Carts
            .FirstOrDefaultAsync(c =>
                c.UserId == userId);

        if (cart == null)
        {
            cart = new FlashCart.Domain.Entities.Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();
        }

        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cart.Id &&
                ci.ProductId == request.ProductId);

        if (existingCartItem != null)
        {
            var newQuantity =
                existingCartItem.Quantity + request.Quantity;

            if (newQuantity > 100)
            {
                return BadRequest(new
                {
                    message = "Cart quantity cannot exceed 100."
                });
            }

            existingCartItem.Quantity = newQuantity;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product quantity updated in cart.",
                cartItemId = existingCartItem.Id,
                productId = existingCartItem.ProductId,
                quantity = existingCartItem.Quantity
            });
        }

        var cartItem = new FlashCart.Domain.Entities.CartItem
        {
            CartId = cart.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        _context.CartItems.Add(cartItem);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(AddToCart),
            new
            {
                cartItemId = cartItem.Id
            },
            new
            {
                message = "Product added to cart.",
                cartItemId = cartItem.Id,
                productId = cartItem.ProductId,
                quantity = cartItem.Quantity
            });
    }

[HttpGet]
public async Task<IActionResult> GetCart()
{
    var userIdValue = User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    var cart = await _context.Carts
        .FirstOrDefaultAsync(c =>
            c.UserId == userId);

    if (cart == null)
    {
        return Ok(new CartResponseDto
        {
            CartId = 0,
            Items = new List<CartItemResponseDto>(),
            Subtotal = 0,
            Tax = 0,
            TaxRate = CartPricingService.TaxRate * 100,
            Total = 0
        });
    }

    var cartItems = await _context.CartItems
        .Where(ci => ci.CartId == cart.Id)
        .Include(ci => ci.Product)
        .ToListAsync();

    var items = cartItems
        .Select(ci => new CartItemResponseDto
        {
            CartItemId = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            Price = ci.Product.Price,
            Quantity = ci.Quantity,
            Subtotal = _cartPricingService.CalculateSubtotal(
                ci.Product.Price,
                ci.Quantity)
        })
        .ToList();

    var subtotal = items.Sum(
        item => item.Subtotal);

    var tax = _cartPricingService.CalculateTax(
        subtotal);

    var total = _cartPricingService.CalculateTotal(
        subtotal,
        tax);

    var response = new CartResponseDto
    {
        CartId = cart.Id,
        Items = items,
        Subtotal = subtotal,
        Tax = tax,
        TaxRate = CartPricingService.TaxRate * 100,
        Total = total
    };

    return Ok(response);
}
[HttpPut("items/{cartItemId}")]
public async Task<IActionResult> UpdateQuantity(
    int cartItemId,
    UpdateCartItemQuantityRequestDto request)
{
    var userIdValue = User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    var cartItem = await _context.CartItems
        .Include(ci => ci.Cart)
        .FirstOrDefaultAsync(ci =>
            ci.Id == cartItemId);

    if (cartItem == null)
    {
        return NotFound(new
        {
            message = "Cart item not found."
        });
    }

    if (cartItem.Cart.UserId != userId)
    {
        return Forbid();
    }

    cartItem.Quantity = request.Quantity;

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Cart item quantity updated.",
        cartItemId = cartItem.Id,
        productId = cartItem.ProductId,
        quantity = cartItem.Quantity
    });
}

[HttpDelete("items/{cartItemId}")]
public async Task<IActionResult> RemoveItem(int cartItemId)
{
    var userIdValue = User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdValue, out var userId))
    {
        return Unauthorized();
    }

    var cartItem = await _context.CartItems
        .Include(ci => ci.Cart)
        .FirstOrDefaultAsync(ci =>
            ci.Id == cartItemId);

    if (cartItem == null)
    {
        return NotFound(new
        {
            message = "Cart item not found."
        });
    }

    if (cartItem.Cart.UserId != userId)
    {
        return Forbid();
    }

    _context.CartItems.Remove(cartItem);

    await _context.SaveChangesAsync();

    return NoContent();
}





}