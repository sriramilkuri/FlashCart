using System.Net;
using System.Net.Http.Json;
using FlashCart.CartService.Application.Common.Interfaces;
using FlashCart.CartService.Application.Product;

namespace FlashCart.CartService.Infrastructure.Product;

public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductResponse?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"api/Products/{productId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<ProductResponse>(
                cancellationToken);
    }
}