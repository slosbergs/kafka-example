using Avro.Specific;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using EventBus.Sdk.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
namespace EventBus.Sdk.Producer;

public interface IEventProducer
{
    Task<DeliveryReport> ProduceAsync<T>(string topic, T data, CloudEvent message, string partitionKey, CancellationToken cancellationToken = default);
    Task<DeliveryReport> ProduceAsync<T>(string topic, T data, CloudEvent message);
}

/// <summary>
/// native kafka producer
/// </summary>
public class KafkaProducer : IEventProducer, IDisposable
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly ISchemaRegistryClient srClient;
    private readonly ProducerConfig producerConfig;
    private readonly IProducer<string, byte[]> _producer;
    private readonly JsonEventFormatter formatter = new JsonEventFormatter(new JsonSerializerOptions() { },
        new JsonDocumentOptions());

    public KafkaProducer(ILogger<KafkaProducer> logger, IOptions<EventBusConfig> evbConfigOptions, ISchemaRegistryClient srClient)
    {
        _logger = logger;
        this.srClient = srClient;
        producerConfig = evbConfigOptions.Value.ProducerConfig;

        _producer = new ProducerBuilder<string, byte[]>(producerConfig)
            .SetLogHandler(LogHandler)
            .Build();
    }

    private void LogHandler(IProducer<string, byte[]> producer, LogMessage message)
    {
        if (message.Level < SyslogLevel.Error)
            _logger.LogCritical(message.Message);

        if (message.Level == SyslogLevel.Error)
            _logger.LogError(message.Message);

        _logger.LogInformation(message.Message);
    }

    public async Task<DeliveryReport> ProduceAsync<T>(string topic, T data, CloudEvent message, string partitionKey, CancellationToken cancellationToken = default)
    {
        var valueSerializer = new Chr.Avro.Confluent.AsyncSchemaRegistrySerializer<T>(srClient); 
        var avroBytes = await valueSerializer.SerializeAsync(data!,
                            new SerializationContext(
                                MessageComponentType.Value,
                                topic));

        message.DataContentType = "application/avro";
        message.Data = avroBytes;
        message.SetPartitionKey(partitionKey);
        var kafkaMessage = message.ToKafkaMessage(ContentMode.Structured, formatter);

        using var stream = new MemoryStream(kafkaMessage.Value);
        Console.WriteLine($"Actual serialized size: {stream.Length} bytes");

        var deliveryReport = await _producer.ProduceAsync(topic, kafkaMessage!, cancellationToken);
        _producer.Flush();
        return DeliveryReport.From(deliveryReport.TopicPartitionOffset);
    }

    public Task<DeliveryReport> ProduceAsync<T>(string topic, T data, CloudEvent message)
    {
        return ProduceAsync<T>(topic, data, message, message.Id);
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }

    public AvroSerializer<T> GetSerializer<T>(T message, ISchemaRegistryClient srClient) where T : ISpecificRecord
    {
        return new AvroSerializer<T>(srClient);
    }
}
