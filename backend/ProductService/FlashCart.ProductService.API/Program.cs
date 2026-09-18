using FlashCart.ProductService.Application.Common.Interfaces;
using FlashCart.ProductService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "ProductDatabase")));

builder.Services.AddScoped<IProductDbContext>(
    sp => sp.GetRequiredService<ProductDbContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();