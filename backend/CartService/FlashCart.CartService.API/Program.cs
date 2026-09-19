using FlashCart.CartService.Application.Common.Interfaces;
using FlashCart.CartService.Infrastructure.Data;
using FlashCart.CartService.Infrastructure.Product;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CartDbContext>(
    options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(
                "CartDatabase")));

builder.Services.AddScoped<ICartDbContext>(
    sp => sp.GetRequiredService<CartDbContext>());

builder.Services.AddHttpClient<IProductClient, ProductClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration[
                "ProductService:BaseUrl"]!);

        client.Timeout =
            TimeSpan.FromSeconds(5);
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();

app.MapControllers();

app.Run();