using FlashCart.Application.DTOs.Auth;
using FlashCart.Application.Interfaces;
using FlashCart.Infrastructure.Data;
using FlashCart.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FlashCart.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FlashCartDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
private readonly IJwtTokenService _jwtTokenService;
  public AuthController(
    FlashCartDbContext context,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService)
{
    _context = context;
    _passwordHasher = passwordHasher;
    _jwtTokenService = jwtTokenService;
}

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var email = request.Email.Trim().ToLower();

var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email); 

     if (existingUser != null)
        {
            return Conflict(new
            {
                message = "Email is already registered."
            });
        }

         var passwordHash = _passwordHasher
            .HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            Role = "Customer"
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

       var response = new RegisterResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

       return CreatedAtAction(
            nameof(Register),
            new { id = user.Id },
            response);

     }

     [HttpPost("login")]
public async Task<IActionResult> Login(
    LoginRequestDto request)
{
    var email = request.Email.Trim().ToLower();

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    var passwordIsValid = _passwordHasher
        .VerifyPassword(
            request.password,
            user.PasswordHash);

    if (!passwordIsValid)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    var token = _jwtTokenService
        .GenerateToken(user);

    var response = new LoginResponseDto
    {
        Token = token,
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    };

    return Ok(response);
}
}