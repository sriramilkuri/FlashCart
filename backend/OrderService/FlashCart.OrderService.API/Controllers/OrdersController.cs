using System.Security.Claims;
using FlashCart.OrderService.Application.Common.Interfaces;
using FlashCart.OrderService.Application.Orders;
using FlashCart.OrderService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderDbContext _dbContext;
    private readonly IInventoryClient _inventoryClient;

 public OrdersController(
    IOrderDbContext dbContext,
    IInventoryClient inventoryClient)
{
    _dbContext = dbContext;
    _inventoryClient = inventoryClient;
}


    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request)
    {
        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        if (request.Items == null ||
            request.Items.Count == 0)
        {
            return BadRequest(new
            {
                message = "Order must contain at least one item."
            });
        }


        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Quantity must be greater than zero."
                });
            }
        }


        var userId = int.Parse(userIdClaim.Value);


        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow
        };


        /*
         * Product prices will eventually come from
         * Product Service.
         *
         * For the first version, we will use a temporary
         * price so we can test Order Service independently.
         */
        decimal total = 0;


        foreach (var item in request.Items)
        {
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            orderItem.SetUnitPrice(100);

            total +=
                orderItem.UnitPrice *
                orderItem.Quantity;

            order.OrderItems.Add(orderItem);

            var reservation =
    await _inventoryClient.ReserveAsync(
        item.ProductId,
        item.Quantity);

if (!reservation.Success)
{
    return Conflict(new
    {
        message =
            $"Unable to reserve inventory for Product {item.ProductId}."
    });
}

        }


        order.Total = total;


        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();


        return Ok(new
        {
            message = "Order created successfully.",
            orderId = order.Id,
            total = order.Total
        });
    }

[HttpGet("{orderId:int}")]
public async Task<IActionResult> GetOrder(
    int orderId)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);


    var order = await _dbContext.Orders
        .AsNoTracking()
        .Include(x => x.OrderItems)
        .SingleOrDefaultAsync(
            x =>
                x.Id == orderId &&
                x.UserId == userId);


    if (order == null)
    {
        return NotFound(new
        {
            message = "Order not found."
        });
    }


    var response = new OrderResponse
    {
        OrderId = order.Id,
        UserId = order.UserId,
        OrderDate = order.OrderDate,
        Total = order.Total,
        Status = order.Status.ToString()
    };


    foreach (var item in order.OrderItems)
    {
        response.Items.Add(
            new OrderItemResponse
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal =
                    item.UnitPrice * item.Quantity
            });
    }


    return Ok(response);
}

[HttpPost("{orderId:int}/cancel")]
public async Task<IActionResult> CancelOrder(
    int orderId)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);


    var order = await _dbContext.Orders
        .SingleOrDefaultAsync(
            x =>
                x.Id == orderId &&
                x.UserId == userId);


    if (order == null)
    {
        return NotFound(new
        {
            message = "Order not found."
        });
    }


    try
    {
        order.Cancel();

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Order cancelled successfully."
        });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new
        {
            message = ex.Message
        });
    }
}


}