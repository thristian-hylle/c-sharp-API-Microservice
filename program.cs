using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register PostgreSQL database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=productsdb;Username=postgres;Password=postgres"));

// Register our Kafka service
builder.Services.AddSingleton<KafkaService>();

var app = builder.Build();

// Create the database automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// GET all products
app.MapGet("/products", async (AppDbContext db) =>
{
    return await db.Products.ToListAsync();
});

// GET one product by id
app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
        return Results.NotFound();

    return Results.Ok(product);
});

// POST create product
app.MapPost("/products", async (CreateProductRequest request, AppDbContext db, KafkaService kafka) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
        return Results.BadRequest("Name and price must be valid.");

    var product = new Product
    {
        Name = request.Name,
        Price = request.Price,
        CreatedAtUtc = DateTime.UtcNow
    };

    // Save to PostgreSQL
    db.Products.Add(product);
    await db.SaveChangesAsync();

    // Publish event to Kafka
    await kafka.PublishProductCreated(product);

    return Results.Created($"/products/{product.Id}", product);
});

app.Run();