using CloudNative.CloudEvents;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Middlewares.Serializer;

namespace KafkaFlow_demo.Extensions;

public static class ProducerConfigurationBuilderExtensions
{
    /// <summary>
    /// Registers a middleware to serialize avro messages using schema registry
    /// </summary>
    /// <param name="middlewares">The middleware configuration builder</param>
    /// <param name="config">The avro serializer configuration</param>
    /// <returns></returns>
    public static IProducerMiddlewareConfigurationBuilder AddCloudEventSrAvroSerializer(
        this IProducerMiddlewareConfigurationBuilder middlewares,
        AvroSerializerConfig config = null)
    {
        return middlewares.Add(
            resolver => new SerializerProducerMiddleware(
                new CloudEventAvroSerializer(resolver, config),
                new SchemaRegistryTypeResolver(new ConfluentAvroTypeNameResolver(resolver.Resolve<ISchemaRegistryClient>()))));
    }
}

public class CloudEventAvroSerializer : ISerializer
{
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly AvroSerializerConfig _serializerConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudEventAvroSerializer"/> class.
    /// </summary>
    /// <param name="resolver">The <see cref="IDependencyResolver"/> to be used by the framework</param>
    /// <param name="serializerConfig">Avro serializer configuration</param>
    public CloudEventAvroSerializer(
        IDependencyResolver resolver,
        AvroSerializerConfig serializerConfig = null)
    {
        _schemaRegistryClient =
            resolver.Resolve<ISchemaRegistryClient>() ??
            throw new InvalidOperationException(
                $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

        _serializerConfig = serializerConfig;
    }

    /// <summary>
    /// data is serialized using Confluent.SchemaRegistry.Serdes
    /// with this in place schema registry is used to check validity of the CloudEvent.Data
    /// </summary>
    /// <param name="message"></param>
    /// <param name="output"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        var cloudEvent = (CloudEvent)message;
        var cloudEventData = cloudEvent.Data!;

        return ConfluentSerializerWrapper
            .GetOrCreateSerializer(
                cloudEventData.GetType(),
                () => Activator.CreateInstance(
                    typeof(AvroSerializer<>).MakeGenericType(message.GetType()),
                    _schemaRegistryClient,
                    _serializerConfig))
            .SerializeAsync(cloudEventData, output, context);

    }
}