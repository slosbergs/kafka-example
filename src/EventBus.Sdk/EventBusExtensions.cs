using EventBus.Sdk.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sdk;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, EventBusConfig evbConfig, Action<IEventBusBuilder> configure)
    {
        var builder = new EventBusBuilder(evbConfig);
        configure(builder);
        builder.Build(services);
        return services;
    }
}
