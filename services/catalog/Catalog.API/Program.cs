using System.Reflection;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Context;
using Catalog.Infrastructure.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(),
    Assembly.GetAssembly(typeof(GetProductByIdQuery))));

builder.Services.AddScoped<ICatalogContext,CatalogContext>();

builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IBrandRepository,ProductRepository>();
builder.Services.AddScoped<ITypeRepository,ProductRepository>();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "This is API for Catalog Microservices in E-Commerce Application",
        Contact =new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Abdalla Magdy",
            Email = "abdullamajdy493@gmail.com",
            Url = new Uri("http://any/")
        }
    });
});


var app = builder.Build();

// Data Seeding
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ICatalogContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting database seeding...");

    await CatalogContextSeed.SeedAsync(context.Products);
    await BrandContextSeed.SeedAsync(context.Brands);
    await TypeContextSeed.SeedAsync(context.Types);

    logger.LogInformation("===== DATABASE SEEDING COMPLETED SUCCESSFULLY =====");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while seeding the database.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
