using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Infrastructure.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Contracts.Services;
using Infrastructure.Services;

namespace Ordering.Infrastructure;

public static class ConfigurateService
{
    public static IServiceCollection AddInfrastructureServices ( this IServiceCollection services, IConfiguration configuration )
    {
        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
                builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
        });
        services.AddScoped<OrderContextSeed>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBase<,,>));
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));
        return services;
    }
}
