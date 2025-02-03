using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvroBlogExamples.EventBus;

public static class EventBusExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, EventBusConfig evbConfig, Action<IEventBusBuilder> configure)
    {
        var builder = new EventBusBuilder(evbConfig);
        configure(builder);
        builder.Build(services);
        return services;
    }
}
