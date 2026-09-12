

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCart.API.Controllers;

[ApiController]
[Authorize(policy:  "AdminOnly")]
[Route("api/[controller]")]


public class AdminController : ControllerBase
{
     [HttpGet]
    public IActionResult GetAdminData()
    {
        return Ok(new
        {
            message = "Welcome, Admin."
        });
    }
}