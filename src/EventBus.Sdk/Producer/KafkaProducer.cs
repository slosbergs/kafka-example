using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using EventBus.Sdk.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace EventBus.Sdk.Producer;

public interface IEventProducer
{
    Task<DeliveryReport> ProduceAsync(string topic, CloudEvent message, string partitionKey);
    Task<DeliveryReport> ProduceAsync(string topic, CloudEvent message);
    Task<DeliveryReport> ProduceAsync(CloudEvent message);
}

public class KafkaProducer : IEventProducer, IDisposable
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly EvbProducerConfig _config;
    private readonly IProducer<string, byte[]> _producer;
    private readonly JsonEventFormatter formatter = new JsonEventFormatter(new JsonSerializerOptions() { },
        new JsonDocumentOptions());

    public KafkaProducer(ILogger<KafkaProducer> logger, EventBusConfig config)
    {
        _logger = logger;
        _config = config.KafkaProducer;
        _config.BootstrapServers = config.BootstrapServers;
        _config.SecurityProtocol = config.Security?.SecurityProtocol ?? SecurityProtocol.Plaintext;
        _config.SaslMechanism = config.Security?.SaslMechanism;

        _producer = new ProducerBuilder<string, byte[]>(_config)
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

    public async Task<DeliveryReport> ProduceAsync(string topic, CloudEvent message, string partitionKey)
    {
        message.SetPartitionKey(partitionKey);
        var kafkaMessage = message.ToKafkaMessage(ContentMode.Structured, formatter);

        var deliveryReport = await _producer.ProduceAsync(_config.Topic, kafkaMessage);
        _producer.Flush();
        return DeliveryReport.From(deliveryReport.TopicPartitionOffset);
    }

    public Task<DeliveryReport> ProduceAsync(string topic, CloudEvent message)
    {
        return ProduceAsync(topic, message, message.Id);
    }

    public Task<DeliveryReport> ProduceAsync(CloudEvent message)
    {
        return ProduceAsync(_config.Topic, message, message.Id);
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}
