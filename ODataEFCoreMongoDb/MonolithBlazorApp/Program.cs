using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Client;
using Microsoft.OData.ModelBuilder;
using MonolithBlazorApp.Infrastructure;
using MonolithBlazorApp.UI.Components;
using OData.Api.IoC;
using OData.Api.Models;
using OData.Api.Services;
using OtherDomain.EFCore.DbContexts;
using OtherDomain.EFCore.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configuration de la connexion MongoDB
// Aspire or docker run -d -p 27017:27017 --name mongodb mongo:latest
var mongoConnectionString = builder.Configuration.GetConnectionString("mongo")
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = "ODataDemo";

///

// Other DbContext creation
builder.Services.AddDbContext<OtherDomainDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, mongoDatabaseName));

// OtherScan the DbContext for DbSet<TEntity> properties and register OData controllers for each entity type
var otherModelBuilder = new ODataConventionModelBuilder();
var otherODataEntityTypes = otherModelBuilder.AutoRegisterEntities<OtherDomainDbContext>();

// Register the EFCoreDataService as the implementation for IDataService
string otherKeyedServiceName = nameof(OtherDomainDbContext);
builder.Services.AddKeyedScoped<IDataService, EFCoreDataService<OtherDomainDbContext>>(otherKeyedServiceName);

// Register OData controllers for each entity type
string otherRoutePrefix = "otherOdata";
builder.Services
  .AddControllers()
  .AddDefaultODataDynamicControllerFeature(otherKeyedServiceName, otherODataEntityTypes, otherModelBuilder.GetEdmModel(), otherRoutePrefix);

///

// Add services to the container.
builder.Services.AddRazorComponents();

///

builder.Services.AddScoped<OtherDomainODataServiceContext>(serviceProvider =>
{
  var nav = serviceProvider.GetRequiredService<NavigationManager>();
  string? baseUrl = nav?.BaseUri;
  if (string.IsNullOrWhiteSpace(baseUrl))
    throw new InvalidOperationException("Configuration value App:BaseUrl is missing or empty");


  Uri baseServiceUri = new Uri(new Uri(baseUrl), $"/{otherRoutePrefix}");  
  return new OtherDomainODataServiceContext(baseServiceUri);
});

///

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.UseAuthorization();

app.MapControllers();

/// 

using (var scope = app.Services.CreateScope())
{
  using var context = scope.ServiceProvider.GetRequiredService<OtherDomainDbContext>();
  context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
  await OtherDomainDbContextExtensions.SeedDataAsync(context);
}

Console.WriteLine("\n🚀 OData API Started - Available endpoints:");
Console.WriteLine($"   GET /{otherRoutePrefix}/$metadata");
foreach (var entityType in otherODataEntityTypes)
{
  var entitySetName = entityType.SetName;
  Console.WriteLine($"   GET /{otherRoutePrefix}/{entitySetName}");
}
Console.WriteLine();


///

app.Run();
