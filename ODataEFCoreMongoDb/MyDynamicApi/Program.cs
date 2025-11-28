using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using MyData.EFCore.DbContexts;
using MyData.EFCore.Seeding;
using MyDynamicApi.Services;

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
var entityTypes = ODataHelper.AutoRegisterEntities(modelBuilder, typeof(AppDbContext));

builder.Services.AddScoped<IDataService, DataService>();

// Génération dynamique des contrôleurs à la volée
var controllerFactory = new DynamicODataControllerFactory(entityTypes);
builder.Services.AddControllers()
    .ConfigureApplicationPartManager(apm =>
    {
        apm.FeatureProviders.Add(controllerFactory);
    })
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(1000)
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
    using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    await SeedDataHelper.SeedDataAsync(context);
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

