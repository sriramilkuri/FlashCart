using System.Net.Http.Json;
using FlashCart.OrderService.Application.Common.Interfaces;
using FlashCart.OrderService.Application.Inventory;

namespace FlashCart.OrderService.Infrastructure.Inventory;

public class InventoryClient : IInventoryClient
{
    private readonly HttpClient _httpClient;

    public InventoryClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<ReserveInventoryResponse> ReserveAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            productId,
            quantity
        };


        var response =
            await _httpClient.PostAsJsonAsync(
                "api/Inventory/reserve",
                request,
                cancellationToken);


        if (!response.IsSuccessStatusCode)
        {
            return new ReserveInventoryResponse
            {
                Success = false,
                Message =
                    $"Inventory Service returned {(int)response.StatusCode}."
            };
        }


        return new ReserveInventoryResponse
        {
            Success = true,
            Message = "Inventory reserved."
        };
    }


    public async Task<ReleaseInventoryResponse> ReleaseAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            productId,
            quantity
        };


        var response =
            await _httpClient.PostAsJsonAsync(
                "api/Inventory/release",
                request,
                cancellationToken);


        if (!response.IsSuccessStatusCode)
        {
            return new ReleaseInventoryResponse
            {
                Success = false,
                Message =
                    $"Inventory Service returned {(int)response.StatusCode}."
            };
        }


        return new ReleaseInventoryResponse
        {
            Success = true,
            Message = "Inventory released."
        };
    }
}