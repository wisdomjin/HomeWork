var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.IndexProfitAPI>("indexprofitapi");

builder.AddProject<Projects.IndexProfitWebApp>("indexprofirwebapp");

builder.Build().Run();
