using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace pefi.persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPeFiPersistance(this IServiceCollection services, Action<MongoDbSettings> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<IDataStore, MongoDatastore>();

        services.AddSingleton<IMongoClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var connectionString = options.ConnectionString;

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            return new MongoClient(clientSettings);
        });

        return services;
    }

    public static IServiceCollection AddPeFiPersistance(this IServiceCollection services, string connectionString)
    {
        return AddPeFiPersistance(services, _ => new MongoDbSettings(connectionString));
    }
}
