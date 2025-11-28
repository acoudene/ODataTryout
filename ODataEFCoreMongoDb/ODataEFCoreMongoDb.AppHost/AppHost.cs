// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MyApi>("myapi");

builder.AddProject<Projects.MyDynamicApi>("mydynamicapi");

builder.Build().Run();
