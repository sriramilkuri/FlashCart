using System.Security.Claims;
using FlashCart.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlashCart.Application.DTO.Orders;
using FlashCart.Domain.Services;
using FlashCart.Domain.Enums;
using FlashCart.Infrastructure.Messaging;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly FlashCartDbContext _dbContext;
    private readonly OrderStateMachine _orderStateMachine;
    private readonly RabbitMqPublisher _rabbitMQPublisher;
    public OrdersController(FlashCartDbContext context, OrderStateMachine orderStateMachine, RabbitMqPublisher rabbitMQPublisher)
    {
        _dbContext = context;
        _orderStateMachine = orderStateMachine;
        _rabbitMQPublisher = rabbitMQPublisher;

    }





[HttpPut("{orderId}/status")]
public async Task<IActionResult> UpdateStatus(
    int orderId,
    UpdateOrderStatusRequestDto request)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);

    var order = await _dbContext.Orders
        .SingleOrDefaultAsync(o =>
            o.Id == orderId &&
            o.UserId == userId);

    if (order == null)
    {
        return NotFound(new
        {
            message = "Order not found."
        });
    }

    var previousStatus = order.Status;

    try
    {
        switch (request.Status)
        {
            case OrderStatus.Confirmed:
                order.Confirm();
                break;

            case OrderStatus.Cancelled:
                order.Cancel();
                break;

            case OrderStatus.Completed:
                order.Complete();
                break;

            default:
                return BadRequest(new
                {
                    message = "Invalid order status."
                });
        }
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new
        {
            message = ex.Message,
            currentStatus = previousStatus.ToString(),
            requestedStatus = request.Status.ToString()
        });
    }

    await _dbContext.SaveChangesAsync();

    return Ok(new
    {
        orderId = order.Id,
        previousStatus = previousStatus.ToString(),
        currentStatus = order.Status.ToString()
    });
}

[HttpGet]
public async Task<IActionResult> GetOrderHistory()
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);

    var orders = await _dbContext.Orders
        .AsNoTracking()
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.OrderDate)
        .Select(o => new OrderHistoryResponseDto
        {
            OrderId = o.Id,
            OrderDate = o.OrderDate,
            Total = o.Total,
            Status = o.Status.ToString(),

            Items = o.OrderItems
                .Select(oi => new OrderHistoryItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                })
                .ToList()
        })
        .ToListAsync();

    return Ok(orders);
}

[HttpGet("{orderId:int}")]
public async Task<IActionResult> GetOrderDetails(int orderId)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);

Console.WriteLine(
    $"Order ID: {orderId}, User ID: {userIdClaim?.Value}");

    var order = await _dbContext.Orders
        .AsNoTracking()
        .Where(o =>
            o.Id == orderId &&
            o.UserId == userId)
        .Select(o => new OrderDetailsResponseDto
        {
            OrderId = o.Id,
            OrderDate = o.OrderDate,
            Total = o.Total,
            Status = o.Status.ToString(),

            Items = o.OrderItems
                .Select(oi => new OrderDetailsItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    LineTotal = oi.UnitPrice * oi.Quantity
                })
                .ToList()
        })
        .SingleOrDefaultAsync();

    if (order == null)
    {
        return NotFound(new
        {
            message = "Order not found."
        });
    }

    return Ok(order);
}

[HttpPost("{orderId:int}/cancel")]
public async Task<IActionResult> CancelOrder(int orderId)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    var userId = int.Parse(userIdClaim.Value);

    var order = await _dbContext.Orders
        .SingleOrDefaultAsync(o =>
            o.Id == orderId &&
            o.UserId == userId);

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

