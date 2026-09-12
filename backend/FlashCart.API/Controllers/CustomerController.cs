using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCustomerData()
    {
        return Ok(new
        {
            message = "Welcome, Customer."
        });
    }
}