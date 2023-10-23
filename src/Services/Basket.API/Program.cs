using Basket.API.Extensions;
using Common.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
Log.Information("Starting Basket API Up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();
    //Add services to the container
    builder.Services.ConfiguretServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Customer Minimal API v1");
    });

    //app.UseHttpsRedirection(); //Production only

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}

