var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CareNavigatorSparrow_Web>("web");

builder.Build().Run();
