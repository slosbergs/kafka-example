using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;

internal class MyCloudEventProducer
{
    private readonly IMessageProducer<CloudEvent> _kfProducer;
    private readonly AvroSerializerConfig _serializerConfig = new()
    { AutoRegisterSchemas = true, SubjectNameStrategy = SubjectNameStrategy.TopicRecord };
    private readonly ISchemaRegistryClient _schemaRegistry;
    private readonly SchemaRegistryConfig _schemaRegistryConfig;

    public MyCloudEventProducer(IMessageProducer<CloudEvent> messageProducer,
        ISchemaRegistryClient schemaRegistryClient,
        SchemaRegistryConfig schemaRegistryConfig)
    {
        _kfProducer = messageProducer;
        _schemaRegistry = schemaRegistryClient;
        _schemaRegistryConfig = schemaRegistryConfig;
    }

    public async Task<DeliveryReport> Produce(string topic, object messageKey, CloudEvent cloudEvent)
    {
        ArgumentNullException.ThrowIfNull(cloudEvent.Data, nameof(cloudEvent.Data));

        var cloudEventData = cloudEvent.Data;
        var valueSerializer = GetValueSerializer(cloudEventData);
        cloudEvent.Data = await valueSerializer.SerializeAsync(
                            cloudEventData,
                            new SerializationContext(
                                MessageComponentType.Value,
                                topic));

        var schemaId = (await _schemaRegistry.GetLatestSchemaAsync(SubjectNameStrategy.Topic.ConstructValueSubjectName(topic))).Id;
        cloudEvent.DataSchema = new Uri($"{_schemaRegistryConfig.Url}/schemas/ids/{schemaId}/schema/");

        Message<string?, byte[]> message = cloudEvent.ToKafkaMessage(ContentMode.Binary, new JsonEventFormatter());
        var delivery = await _kfProducer.ProduceAsync(topic, message.Key, message.Value, (IMessageHeaders)message.Headers);
        return DeliveryReport.From(delivery.TopicPartitionOffset);
    }

    private IAsyncSerializer<T> GetValueSerializer<T>(T cloudEventData)
    {
        return new AvroSerializer<T>(_schemaRegistry, _serializerConfig);
    }
}

public record DeliveryReport
{
    public string? Topic { get; private set; }
    public int Partition { get; private set; }
    public long Offset { get; private set; }

    internal static DeliveryReport From(Confluent.Kafka.TopicPartitionOffset topicPartitionOffset)
    {
        return new()
        {
            Topic = topicPartitionOffset.Topic,
            Partition = topicPartitionOffset.Partition,
            Offset = topicPartitionOffset.Offset
        };
    }
}