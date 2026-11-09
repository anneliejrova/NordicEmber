using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Data;
using NordicEmber.Api.Features.Categories;
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

builder.Services.AddScoped<CategoryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapProductEndpoints();

app.MapCategoryEndpoints();

app.Run();