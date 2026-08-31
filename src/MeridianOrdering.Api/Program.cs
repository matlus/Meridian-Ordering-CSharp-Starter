var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var application = builder.Build();

application.MapControllers();

application.Run();
