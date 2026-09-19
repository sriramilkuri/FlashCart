using FlashCart.ProductService.Application.Common.Interfaces;
using FlashCart.ProductService.Application.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductDbContext _context;

    public ProductsController(
        IProductDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        int? cursor = null,
        int pageSize = 20,
        int direction = 1,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                message =
                    "Page size must be between 1 and 100."
            });
        }

        if (minPrice < 0 || maxPrice < 0)
        {
            return BadRequest(new
            {
                message =
                    "Price cannot be negative."
            });
        }

        if (minPrice.HasValue &&
            maxPrice.HasValue &&
            minPrice > maxPrice)
        {
            return BadRequest(new
            {
                message =
                    "Minimum price cannot be greater than maximum price."
            });
        }

        if (direction != 1 && direction != -1)
        {
            return BadRequest(new
            {
                message =
                    "Direction must be 1 or -1."
            });
        }

        var lastSeenId = cursor ?? 0;

        if (lastSeenId < 0)
        {
            return BadRequest(new
            {
                message =
                    "Cursor cannot be negative."
            });
        }

        var query = _context.Products
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= maxPrice.Value);
        }

        List<ProductDto> products;
        bool hasMore;

        if (direction == 1)
        {
            products = await query
                .Where(p => p.Id > lastSeenId)
                .OrderBy(p => p.Id)
                .Take(pageSize + 1)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            hasMore = products.Count > pageSize;

            if (hasMore)
            {
                products.RemoveAt(pageSize);
            }
        }
        else
        {
            products = await query
                .Where(p => p.Id < lastSeenId)
                .OrderByDescending(p => p.Id)
                .Take(pageSize + 1)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            hasMore = products.Count > pageSize;

            if (hasMore)
            {
                products.RemoveAt(pageSize);
            }

            products.Reverse();
        }

        int? nextCursor = null;
        int? previousCursor = null;

        if (products.Count > 0)
        {
            previousCursor = products[0].Id;
            nextCursor = products[^1].Id;
        }

        bool hasNextPage;
        bool hasPreviousPage;

        if (direction == 1)
        {
            hasNextPage = hasMore;

            if (products.Count > 0)
            {
                hasPreviousPage = await query
                    .AnyAsync(p =>
                        p.Id < products[0].Id);
            }
            else
            {
                hasPreviousPage = false;
            }
        }
        else
        {
            hasPreviousPage = hasMore;

            if (products.Count > 0)
            {
                var lastProductId =
                    products[^1].Id;

                hasNextPage =
                    await query.AnyAsync(
                        p => p.Id > lastProductId);
            }
            else
            {
                hasNextPage = false;
            }
        }

        var response =
            new ProductCursorResponseDto
            {
                Items = products,

                NextCursor = nextCursor,

                PreviousCursor = previousCursor,

                HasNextPage = hasNextPage,

                HasPreviousPage =
                    hasPreviousPage,

                PageSize = pageSize
            };

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(
        int id)
    {
        var product =
            await _context.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CategoryId = product.CategoryId
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        FlashCart.ProductService.Domain.Entities.Product product)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }
}