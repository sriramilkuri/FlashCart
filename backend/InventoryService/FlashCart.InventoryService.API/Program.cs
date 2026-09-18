using FlashCart.InventoryService.Application.Common.Interfaces;
using FlashCart.InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "InventoryDatabase")));

builder.Services.AddScoped<IInventoryDbContext>(
    sp => sp.GetRequiredService<InventoryDbContext>());

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