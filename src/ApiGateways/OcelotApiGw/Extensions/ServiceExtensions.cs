
using Contracts.Identity;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Shared.Configurations;
using System.Text;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
       IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        services.AddSingleton(jwtSettings);
        //var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
        //    .Get<EventBusSettings>();
        //services.AddSingleton(eventBusSettings);

        //var cacheSettings = configuration.GetSection(nameof(CacheSettings))
        //   .Get<CacheSettings>();
        //services.AddSingleton(cacheSettings);

        //var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
        //.Get<GrpcSettings>();
        //services.AddSingleton(grpcSettings);

        return services;
    }

    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration);
        services.AddTransient<ITokenService, TokenService>();
        services.AddJwtAuthentication();
    }
    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        if (settings == null || string.IsNullOrEmpty(settings.Key))
            throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RefreshBeforeValidation = false,
        };
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParameters;
        });
        return services;
    }
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });

        //builder.AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader());
    }
    //public static IServiceCollection ConfiguretServices ( this IServiceCollection services ) =>
    //    services.AddScoped<IBasketRepository, BasketRepository>()
    //    .AddTransient<ISerializeService, SerializeService>();

    //public static void ConfigureRedis ( this IServiceCollection services, IConfiguration configuration )
    //{
    //    var settings = services.GetOptions<CacheSettings>("CacheSettings");
    //    //redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
    //    if (string.IsNullOrWhiteSpace(settings.ConnectionString))
    //    {
    //        throw new ArgumentNullException("Redis Connection string is not configured");
    //    }
    //    else
    //    {
    //        services.AddStackExchangeRedisCache(options =>
    //        {
    //            options.Configuration = settings.ConnectionString;
    //        });
    //    }
    //}
    //public static IServiceCollection ConfigureGrpcServices ( this IServiceCollection services)
    //{
    //    var settings =   services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
    //    services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x => x.Address = new Uri(settings.StockUrl));
    //    services.AddScoped<StockItemGrpcService>();
    //    return services;
    //}
    //public static void ConfigureMassTransit ( this IServiceCollection services )
    //{
    //    var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
    //    if (settings == null || string.IsNullOrEmpty(settings.HostAddress) ||
    //        string.IsNullOrEmpty(settings.HostAddress)) throw new ArgumentNullException("EventBusSettings is not configured!");

    //    var mqConnection = new Uri(settings.HostAddress);

    //    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
    //    services.AddMassTransit(config =>
    //    {
    //        config.UsingRabbitMq(( ctx, cfg ) =>
    //        {
    //            cfg.Host(mqConnection);
    //        });
    //        // Publish submit order message, instead of sending it to a specific queue directly.
    //        config.AddRequestClient<IBasketCheckoutEvent>();
    //    });
    //}

}
