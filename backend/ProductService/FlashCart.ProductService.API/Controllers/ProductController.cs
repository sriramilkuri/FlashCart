using FlashCart.ProductService.Application.Common.Interfaces;
using FlashCart.ProductService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductDbContext _dbContext;

    public ProductsController(
        IProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _dbContext.Products
            .AsNoTracking()
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(
        int id)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        Product product)
    {
        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }
}