using EventBus.Sdk.Configuration;
using EventBus.Sdk.Consumer;
using EventBus.Sdk.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sdk;

public interface IEventBusBuilder
{
    IEventBusBuilder WithKafkaProducer();
    IEventBusBuilder WithKafkaConsumer<THandler>() where THandler : class, ICloudEventHandler;
    void Build(IServiceCollection services);
}

public class EventBusBuilder : IEventBusBuilder
{
    private EventBusConfig _config;
    private Type _eventHandlerType;

    public EventBusBuilder(EventBusConfig evbConfig)
    {
        _config = evbConfig;
    }

    public IEventBusBuilder WithKafkaProducer()
    {
        if (_config.KafkaProducer == null)
            throw new InvalidOperationException("KafkaProducer not configured");

        return this;
    }

    public IEventBusBuilder WithKafkaConsumer<THandler>() where THandler : class, ICloudEventHandler
    {
        if (_config.KafkaConsumer == null)
            throw new InvalidOperationException("KafkaConsumer not configured");

        _eventHandlerType = typeof(THandler);
        return this;
    }

    public void Build(IServiceCollection services)
    {
        services.AddSingleton(_config);

        // Register Kafka Producer
        services.AddTransient<IEventProducer, KafkaProducer>();

        // Register the event handler
        if (_eventHandlerType != null)
        {
            services.AddScoped(typeof(ICloudEventHandler), _eventHandlerType);
        }

        // Register Kafka Consumer as a Background Service
        services.AddHostedService<KafkaConsumer>();
    }
}
