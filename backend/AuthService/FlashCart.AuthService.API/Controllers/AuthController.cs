using FlashCart.AuthService.Application.Auth;
using FlashCart.AuthService.Application.Common.Interfaces;
using FlashCart.AuthService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        IAuthDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto request)
    {
        var email =
            request.Email.Trim().ToLowerInvariant();

        var existingUser =
            await _dbContext.Users
                .AnyAsync(x => x.Email == email);

        if (existingUser)
        {
            return Conflict(new
            {
                message = "Email is already registered."
            });
        }

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            Role = "Customer",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var response = new RegisterResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };

        return Created(
            $"api/Auth/{user.Id}",
            response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto request)
    {
        var email =
            request.Email.Trim().ToLowerInvariant();

        var user =
            await _dbContext.Users
                .SingleOrDefaultAsync(
                    x => x.Email == email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var token =
            _jwtTokenService.GenerateToken(user);

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };

        return Ok(response);
    }
}