using Avro;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using EventBus.Sdk.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EventBus.Sdk.Consumer;
public class KafkaConsumer : BackgroundService
{
    private readonly ConsumerConfig _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, byte[]> _consumer;
    private readonly JsonEventFormatter formatter = new JsonEventFormatter(new JsonSerializerOptions() { },
    new JsonDocumentOptions() { });

    public KafkaConsumer(ILogger<KafkaConsumer> logger, EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = eventBusConfig.KafkaConsumer;
        _config.GroupId ??= "consumer__" + Guid.NewGuid().ToString();
        _config.GroupInstanceId ??= Guid.NewGuid().ToString();
        _config.BootstrapServers = eventBusConfig.BootstrapServers;

        // FIXME add OAuth authentication

        _consumer = new ConsumerBuilder<string, byte[]>(_config)
            .SetLogHandler(OnLog)
            .SetErrorHandler(OnError)
            .Build();

        var topics = eventBusConfig.KafkaConsumer.Topics!.Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(t => "^" + Regex.Escape(t).Replace("\\*", ".*") + "$");
        _consumer.Subscribe(topics);
    }

    private void OnError(IConsumer<string, byte[]> consumer, Error error)
    {
        _logger.LogError("IsLocalError: {IsLocalError}, Error: {Code}, {Reason}", error.IsLocalError, error.Code, error.Reason);
    }

    private void OnLog(IConsumer<string, byte[]> producer, LogMessage message)
    {
        if (message.Level < SyslogLevel.Error)
            _logger.LogCritical(message.Message);

        if (message.Level == SyslogLevel.Error)
            _logger.LogError(message.Message);

        _logger.LogInformation(message.Message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer started...");
        _logger.LogInformation("Consuming: {topics}", string.Join(", ", _consumer.Subscription));
        Headers? headers = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(100);
                if (consumeResult is null)
                {
                    _logger.LogDebug("nothing received");
                    await Task.Delay(1000);
                    continue;
                }

                var consumedMessage = consumeResult.Message;
                headers = consumeResult.Message.Headers;
                var cloudEventMessage = consumedMessage.ToCloudEvent(formatter);

                _logger.LogInformation($"Received message: {cloudEventMessage.Id}, {cloudEventMessage.Type}");

                // Resolve and call the event handler
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ICloudEventHandler>();
                await handler.HandleMessageAsync(cloudEventMessage);

                _consumer.Commit();
            }
            catch (Exception ex)
            {
                var headerList = headers.Select(h => $"{h.Key}:{Convert.ToString(h.GetValueBytes())}").ToArray();
                headers = null;
                _logger.LogError(ex, $"Error processing Kafka message. {@headers}", headerList);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka Consumer stopping...");
        _consumer.Close();
        return base.StopAsync(cancellationToken);
    }
}
