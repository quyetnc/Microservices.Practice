using Common.Logging;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information("Starting Inventory API Up");

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
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

