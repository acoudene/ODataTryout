// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using OData.Api.IoC;
using OData.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Domain.EFCore.DbContexts;
using Domain.EFCore.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuration de la connexion MongoDB
// docker run -d -p 27017:27017 --name mongodb mongo:latest
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = "ODataDemo";

// DbContext creation
builder.Services.AddDbContext<DomainDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

// Scan the DbContext for DbSet<TEntity> properties and register OData controllers for each entity type
var modelBuilder = new ODataConventionModelBuilder();
var entityTypes = modelBuilder.AutoRegisterEntities<DomainDbContext>();

// Register the EFCoreDataService as the implementation for IDataService
builder.Services.AddScoped<IDataService, EFCoreDataService<DomainDbContext>>();

// Register OData controllers for each entity type
builder.Services
  .AddControllers()
  .AddDefaultODataDynamicControllerFeature(entityTypes, modelBuilder.GetEdmModel(), "odata");

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

// Seeding initial data
using (var scope = app.Services.CreateScope())
{
  using var context = scope.ServiceProvider.GetRequiredService<DomainDbContext>();
  context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
  await DomainDbContextExtensions.SeedDataAsync(context);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("\n🚀 OData API Started - Available endpoints:");
Console.WriteLine("   GET /odata/$metadata");
foreach (var entityType in entityTypes)
{
  var entitySetName = entityType.Name + "s";
  if (entityType.Name.EndsWith("y"))
    entitySetName = entityType.Name.Substring(0, entityType.Name.Length - 1) + "ies";
  Console.WriteLine($"   GET /odata/{entitySetName}");
}
Console.WriteLine();

app.Run();

