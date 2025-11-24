var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MyApi>("myapi");

builder.Build().Run();
