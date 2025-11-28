// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Domain.EFCore.DbContexts;
using Domain.EFCore.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using OData.Api.IoC;
using OData.Api.Services;
using OtherDomain.EFCore.DbContexts;
using OtherDomain.EFCore.Seeding;

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

///

// DbContext creation
builder.Services.AddDbContext<DomainDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

// Scan the DbContext for DbSet<TEntity> properties and register OData controllers for each entity type
var modelBuilder = new ODataConventionModelBuilder();
var entityTypes = modelBuilder.AutoRegisterEntities<DomainDbContext>();

// Register the EFCoreDataService as the implementation for IDataService
string keyedServiceName = nameof(DomainDbContext);
builder.Services.AddKeyedScoped<IDataService, EFCoreDataService<DomainDbContext>>(keyedServiceName);

// Register OData controllers for each entity type
string routePrefix = "odata";
builder.Services
  .AddControllers()
  .AddDefaultODataDynamicControllerFeature(keyedServiceName, entityTypes, modelBuilder.GetEdmModel(), routePrefix);

///

// Other DbContext creation
builder.Services.AddDbContext<OtherDomainDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

// OtherScan the DbContext for DbSet<TEntity> properties and register OData controllers for each entity type
var otherModelBuilder = new ODataConventionModelBuilder();
var otherEntityTypes = otherModelBuilder.AutoRegisterEntities<OtherDomainDbContext>();

// Register the EFCoreDataService as the implementation for IDataService
string otherKeyedServiceName = nameof(OtherDomainDbContext);
builder.Services.AddKeyedScoped<IDataService, EFCoreDataService<OtherDomainDbContext>>(otherKeyedServiceName);

// Register OData controllers for each entity type
string otherRoutePrefix = "otherOdata";
builder.Services
  .AddControllers()
  .AddDefaultODataDynamicControllerFeature(otherKeyedServiceName, otherEntityTypes, otherModelBuilder.GetEdmModel(), otherRoutePrefix);

///

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

///

// Seeding initial data
using (var scope = app.Services.CreateScope())
{
  using var context = scope.ServiceProvider.GetRequiredService<DomainDbContext>();
  context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
  await DomainDbContextExtensions.SeedDataAsync(context);
}

/// 

using (var scope = app.Services.CreateScope())
{
  using var context = scope.ServiceProvider.GetRequiredService<OtherDomainDbContext>();
  context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
  await OtherDomainDbContextExtensions.SeedDataAsync(context);
}

///

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("\n🚀 OData API Started - Available endpoints:");
Console.WriteLine($"   GET /{routePrefix}/$metadata");
foreach (var entityType in entityTypes)
{
  var entitySetName = entityType.Name + "s";
  if (entityType.Name.EndsWith("y"))
    entitySetName = entityType.Name.Substring(0, entityType.Name.Length - 1) + "ies";
  Console.WriteLine($"   GET /{routePrefix}/{entitySetName}");
}
Console.WriteLine($"   GET /{otherRoutePrefix}/$metadata");
foreach (var entityType in otherEntityTypes)
{
  var entitySetName = entityType.Name + "s";
  if (entityType.Name.EndsWith("y"))
    entitySetName = entityType.Name.Substring(0, entityType.Name.Length - 1) + "ies";
  Console.WriteLine($"   GET /{otherRoutePrefix}/{entitySetName}");
}
Console.WriteLine();

app.Run();

