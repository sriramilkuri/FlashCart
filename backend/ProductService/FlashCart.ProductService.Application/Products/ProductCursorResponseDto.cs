namespace FlashCart.ProductService.Application.Products;

public class ProductCursorResponseDto
{
    public List<ProductDto> Items { get; set; }
        = new();

    public int? NextCursor { get; set; }

    public int? PreviousCursor { get; set; }

    public bool HasNextPage { get; set; }

    public bool HasPreviousPage { get; set; }

    public int PageSize { get; set; }
}