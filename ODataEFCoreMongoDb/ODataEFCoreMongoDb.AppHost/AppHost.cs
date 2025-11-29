// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using ODataEFCoreMongoDb.AppHost.Seeding;

var builder = DistributedApplication.CreateBuilder(args);

const string databaseName = "ODataDemo";

var mongoContainer = builder.AddMongoDB("mongo");

var mongoDatabase = mongoContainer
  .AddDatabase(databaseName)
  .SeedOnResourceReady(["categories", "products", "orders", "superHeros"]);

builder.AddProject<Projects.MyApi>("myapi")
  .WithReference(mongoContainer)
  .WithReference(mongoDatabase)
  .WaitFor(mongoDatabase);


builder.AddProject<Projects.MyDynamicApi>("mydynamicapi")
  .WithReference(mongoContainer)
  .WithReference(mongoDatabase)
  .WaitFor(mongoDatabase);

builder.Build().Run();
