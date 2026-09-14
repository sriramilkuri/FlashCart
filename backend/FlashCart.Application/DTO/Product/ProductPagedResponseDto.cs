namespace FlashCart.Application.DTO.Product;

public class ProductPagedResponseDto
{
    public List<ProductDto> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}


