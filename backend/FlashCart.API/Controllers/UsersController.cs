using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        var name = User.FindFirst(
            ClaimTypes.Name)?.Value;

        var email = User.FindFirst(
            ClaimTypes.Email)?.Value;

        return Ok(new
        {
            userId,
            name,
            email
        });
    }
}