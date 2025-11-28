var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MyApi>("myapi");

builder.AddProject<Projects.MyDynamicApi>("mydynamicapi");

builder.Build().Run();
