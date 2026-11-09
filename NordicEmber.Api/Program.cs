using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Common;
using NordicEmber.Api.Data;
using NordicEmber.Api.Features.Products;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddValidation();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) => DbSeeder.Seed(context))
        .UseAsyncSeeding((context, _, cancellationToken) => DbSeeder.SeedAsync(context, cancellationToken)));

builder.Services.AddScoped<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/dev/slug", (string text) => SlugGenerator.Generate(text));
}

app.UseHttpsRedirection();

app.MapProductEndpoints();

app.Run();