using System.Security.Claims;
using FlashCart.CartService.Application.Cart;
using FlashCart.CartService.Application.Common.Interfaces;
using FlashCart.CartService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FlashCart.CartService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartDbContext _dbContext;
    private readonly IProductClient _productClient;

    public CartController(
        ICartDbContext dbContext, IProductClient productClient)
    {
        _dbContext = dbContext;
        _productClient = productClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var cart = await _dbContext.Carts
            .AsNoTracking()
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(
                x => x.UserId == userId.Value);

        if (cart == null)
        {
            return Ok(new CartResponse
            {
                CartId = 0,
                UserId = userId.Value,
                UpdatedAt = DateTime.UtcNow
            });
        }

        var response = new CartResponse
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            UpdatedAt = cart.UpdatedAt
        };

        foreach (var item in cart.CartItems)
        {
            response.Items.Add(
                new CartItemResponse
                {
                    CartItemId = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
        }

        return Ok(response);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        AddCartItemRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        if (request.ProductId <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid product ID."
            });
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(new
            {
                message =
                    "Quantity must be greater than zero."
            });
        }

var product =
    await _productClient.GetProductAsync(
        request.ProductId);

if (product == null)
{
    return NotFound(new
    {
        message = "Product not found."
    });
}
        var cart = await _dbContext.Carts
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(
                x => x.UserId == userId.Value);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Carts.Add(cart);
        }

        var existingItem = cart.CartItems
            .SingleOrDefault(
                x => x.ProductId == request.ProductId);

        if (existingItem != null)
        {
            try
            {
                existingItem.IncreaseQuantity(
                    request.Quantity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        else
        {
            var cartItem = new CartItem
            {
                ProductId = request.ProductId
            };

            try
            {
                cartItem.SetQuantity(
                    request.Quantity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            cart.CartItems.Add(cartItem);
        }

        cart.UpdateTimestamp();

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Item added to cart."
        });
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(
        int productId,
        UpdateCartItemRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        if (productId <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid product ID."
            });
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(new
            {
                message =
                    "Quantity must be greater than zero."
            });
        }

        var cart = await _dbContext.Carts
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(
                x => x.UserId == userId.Value);

        if (cart == null)
        {
            return NotFound(new
            {
                message = "Cart not found."
            });
        }

        var item = cart.CartItems
            .SingleOrDefault(
                x => x.ProductId == productId);

        if (item == null)
        {
            return NotFound(new
            {
                message = "Cart item not found."
            });
        }

        try
        {
            item.SetQuantity(request.Quantity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }

        cart.UpdateTimestamp();

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Cart item updated."
        });
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(
        int productId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var cart = await _dbContext.Carts
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(
                x => x.UserId == userId.Value);

        if (cart == null)
        {
            return NotFound(new
            {
                message = "Cart not found."
            });
        }

        var item = cart.CartItems
            .SingleOrDefault(
                x => x.ProductId == productId);

        if (item == null)
        {
            return NotFound(new
            {
                message = "Cart item not found."
            });
        }

        _dbContext.CartItems.Remove(item);

        cart.UpdateTimestamp();

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Item removed from cart."
        });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var cart = await _dbContext.Carts
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(
                x => x.UserId == userId.Value);

        if (cart == null)
        {
            return Ok(new
            {
                message = "Cart is already empty."
            });
        }

        _dbContext.CartItems.RemoveRange(
            cart.CartItems);

        cart.UpdateTimestamp();

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Cart cleared."
        });
    }

    private int? GetUserId()
    {
        var claim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            return null;

        if (!int.TryParse(
                claim.Value,
                out var userId))
        {
            return null;
        }

        return userId;
    }
}