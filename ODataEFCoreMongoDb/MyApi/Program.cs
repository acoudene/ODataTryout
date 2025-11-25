using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using MyApi.DbContexts;
using MyApi.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuration de la connexion MongoDB
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = "ODataDemo";

// docker run -d -p 27017:27017 --name mongodb mongo:latest

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

// Configuration du modèle OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Product>("Products");
modelBuilder.EntitySet<Category>("Categories");
modelBuilder.EntitySet<Order>("Orders");

builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwagger();
  app.UseSwaggerUI();

}

using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await SeedData(context);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedData(AppDbContext context)
{
  if (!await context.Products.AnyAsync())
  {
    var electronics = new Category { Id = new Guid("ced4043c-104f-44fd-8ffb-18feee86aef5"), Name = "Electronics", Description = "Electronic devices" };
    var books = new Category { Id = new Guid("3f8b0347-ac89-470c-a24b-93cf58a538eb"), Name = "Books", Description = "Books and literature" };

    context.Categories.AddRange(electronics, books);

    context.Products.AddRange(
        new Product { Id = new Guid("d15bbdf5-084b-45c3-a415-a2fad410718f"), Name = "Laptop", Price = 1200.00m, Stock = 15, CategoryId = electronics.Id },
        new Product { Id = new Guid("9add3ed2-c871-42ea-a3db-347fb77ff4c4"), Name = "Smartphone", Price = 800.00m, Stock = 25, CategoryId = electronics.Id },
        new Product { Id = new Guid("0855d688-6529-4c8c-b55d-9f51fb2e9fb9"), Name = "Clean Code", Price = 45.00m, Stock = 50, CategoryId = books.Id },
        new Product { Id = new Guid("e99e36fe-490d-4228-a745-75679cb993db"), Name = "Design Patterns", Price = 55.00m, Stock = 30, CategoryId = books.Id }
    );

    context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

    await context.SaveChangesAsync();
  }
}