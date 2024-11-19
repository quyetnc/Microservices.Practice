using MySqlConnector;
using Product.API.Persistence;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Product.API.Repositories.Interfaces;
using Product.API.Repositories;

namespace Product.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure ( this IServiceCollection services, IConfiguration configuration )
        {
            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureProductDbContext(configuration);
            services.AddInfrastructureServices();
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
            return services;
        }

        private static IServiceCollection ConfigureProductDbContext ( this IServiceCollection services, IConfiguration configuration )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");

            var builder = new MySqlConnectionStringBuilder(connectionString);

            services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
                ServerVersion.AutoDetect(builder.ConnectionString), e =>
                {
                    e.MigrationsAssembly("Product.API");
                    e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                }));
            return services;
        }


        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBase<,,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<IProductRepository, ProductRepository>();
        }
    }
}

