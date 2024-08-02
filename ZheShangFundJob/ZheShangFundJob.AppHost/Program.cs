var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.IndexProfitAPI>("indexprofitapi");

builder.Build().Run();
