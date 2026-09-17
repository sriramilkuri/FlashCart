using FlashCart.NotificationWorker;
using FlashCart.Application.Common.Interfaces;
using FlashCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<FlashCartDbContext>(
    options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(
                "DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(
    sp => sp.GetRequiredService<FlashCartDbContext>());

var host = builder.Build();
host.Run();
