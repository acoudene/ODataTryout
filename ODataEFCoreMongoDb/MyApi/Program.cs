// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Domain.EFCore.DbContexts;
using Domain.EFCore.Entities;
using Domain.EFCore.Seeding;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

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

builder.Services.AddDbContext<DomainDbContext>(options =>
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
    using var context = scope.ServiceProvider.GetRequiredService<DomainDbContext>();
    context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    await DomainDbContextExtensions.SeedDataAsync(context);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

