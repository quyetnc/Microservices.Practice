using Infrastructure.Extensions;
using Inventory.Grpc.Repositories.Interfaces;
using Inventory.Grpc.Repositories;
using MongoDB.Driver;
using Shared.Configurations;
using Inventory.Grpc.Entities;
using Shared.Enums.Inventory;

namespace Inventory.Grpc.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings ( this IServiceCollection services,
       IConfiguration configuration )
        {
            var databaseSettings = configuration.GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();
            services.AddSingleton(databaseSettings);

            return services;
        }
        private static string getMongoConnectionString ( this IServiceCollection services )
        {
            var settings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("DatabaseSettings is not configured");

            var databaseName = settings.DatabaseName;
            var mongodbConnectionString = settings.ConnectionString + "/" + databaseName +
                                          "?authSource=admin";
            return mongodbConnectionString;
        }

        public static void ConfigureMongoDbClient ( this IServiceCollection services )
        {
            services.AddSingleton<IMongoClient>(
                new MongoClient(getMongoConnectionString(services)))
                .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
        }

        public static void AddInfrastructureServices ( this IServiceCollection services )
        {
            services.AddScoped<IInventoryRepository, InventoryRepository>();
        }

        public static IHost MigrateDatabase ( this IHost host )
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var settings = services.GetService<MongoDbSettings>();
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("DatabaseSettings is not configured");

            var mongoClient = services.GetRequiredService<IMongoClient>();
            new InventoryDbSeed()
                .SeedDataAsync(mongoClient, settings)
                .Wait();
            return host;
        }

        public class InventoryDbSeed
        {
            public async Task SeedDataAsync ( IMongoClient mongoClient, MongoDbSettings settings )
            {
                var databaseName = settings.DatabaseName;
                var database = mongoClient.GetDatabase(databaseName);
                var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
                
                if (await inventoryCollection.EstimatedDocumentCountAsync() == 0)
                {
                    await inventoryCollection.InsertManyAsync(GetPreconfiguredInventories());
                }
            }

            private IEnumerable<InventoryEntry> GetPreconfiguredInventories ( )
            {
                return new List<InventoryEntry>
        {
            new()
            {
                Quantity = 10,
                ItemNo = "Lotus"
            },
            new()
            {
                ItemNo = "Cadillac",
                Quantity = 10
            },
        };
            }
        }
    }
}
