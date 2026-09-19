using FlashCart.OrderService.Application.Common.Interfaces;
using FlashCart.OrderService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FlashCart.OrderService.Infrastructure.Inventory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
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

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "OrderDatabase")));


builder.Services.AddScoped<IOrderDbContext>(
    sp => sp.GetRequiredService<OrderDbContext>());

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration[
                "InventoryService:BaseUrl"]!);

        client.Timeout =
            TimeSpan.FromSeconds(5);
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