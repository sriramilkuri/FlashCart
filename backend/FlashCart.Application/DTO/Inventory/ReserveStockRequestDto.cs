using System.ComponentModel.DataAnnotations;

namespace FlashCart.Application.DTO.Inventory;

public class ReserveStockRequestDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}