using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
//builder.Host.UseSerilog(( ctx, lc ) => lc
//.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Exception}{NewLine}")
//.Enrich.FromLogContext()
//.ReadFrom.Configuration(ctx.Configuration));

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console() //LoggerConfiguration
//    .CreateBootstrapLogger();
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    Log.Information("Starting Product API Up");
    //builder.Host.UseSerilog(( context, configuration ) =>
    //{
    //    configuration
    //    .WriteTo.Debug()
    //    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{Exception}{NewLine}")
    //    .Enrich.FromLogContext()
    //    .ReadFrom.Configuration(context.Configuration);
    //});

    builder.Host.AddAppConfigurations();
    //Add services to te container
    builder.Services.AddInfrastructure(builder.Configuration);
    var app = builder.Build();

    app.UseInfrastructure();

    app.MigrateDatabase<ProductContext>(( context, _ ) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    })
        .Run();
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

    //app.UseHttpsRedirection();

    //app.UseAuthorization();

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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}

