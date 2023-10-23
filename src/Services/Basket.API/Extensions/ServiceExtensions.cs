using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfiguretServices ( this IServiceCollection services ) =>
        services.AddScoped<IBasketRepository, BasketRepository>()
        .AddTransient<ISerializeService, SerializerService>();

    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new ArgumentNullException("Redis Connection string is not configured");
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
    }
}
