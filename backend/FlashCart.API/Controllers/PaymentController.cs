using System.Security.Claims;
using FlashCart.Application.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlashCart.Application.DTO.Payments;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;
    

    private readonly RefundService _refundService;

public PaymentsController(
    PaymentService paymentService,
    RefundService refundService)
{
    _paymentService = paymentService;
    _refundService = refundService;
}

    [HttpPost]
public async Task<IActionResult> CreatePayment(
    [FromBody] CreatePaymentRequest request,
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
{
    if (string.IsNullOrWhiteSpace(idempotencyKey))
    {
        return BadRequest(new
        {
            message = "Idempotency-Key header is required."
        });
    }

    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized();

    if (!int.TryParse(userIdClaim.Value, out var userId))
        return Unauthorized();

    try
    {
        var result =
            await _paymentService.ProcessPaymentAsync(
                request.OrderId,
                userId,
                idempotencyKey);

        return Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}

[HttpPost("{paymentId:int}/refund")]
public async Task<IActionResult> RefundPayment(
    int paymentId)
{
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized();

    if (!int.TryParse(
            userIdClaim.Value,
            out var userId))
    {
        return Unauthorized();
    }

    try
    {
        var result =
            await _refundService.RefundPaymentAsync(
                paymentId,
                userId);

        return Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new
        {
            message = ex.Message
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}




}