using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Controllers;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information("Starting Customer API Up");

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString));

    builder.Services.AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
                //.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<ICustomerRepository, CustomerRepository>()
                //.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBase<,,>))
                .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();
    app.MapCustomersAPI();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Customer Minimal API v1");
    });

    //app.UseHttpsRedirection(); //Production only

    app.UseAuthorization();

    app.MapControllers();

    app.SeedCustomerData().Run();
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
    Log.Information("Shut down Customer API complete");
    Log.CloseAndFlush();
}

