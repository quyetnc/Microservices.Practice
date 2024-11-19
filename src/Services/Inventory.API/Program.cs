using Common.Logging;
using Inventory.Product.API.Extensions;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information("Starting Inventory API Up");

try
{
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureMongoDbClient();
   
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapDefaultControllerRoute();

    app.MigrateDatabase().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
    throw;
}
finally
{
    Log.Information("Shut down Inventory API complete");
    Log.CloseAndFlush();
}

