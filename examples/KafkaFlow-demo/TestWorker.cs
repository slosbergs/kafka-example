using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using KafkaFlow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class TestWorker(ILogger<TestWorker> log, IMyProducer myProducer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        CloudEvent cloudEvent = new CloudEvent()
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri("/demo", UriKind.Relative),
            Time = DateTime.UtcNow,
            Data = new DemoEvent() { Id = 1, MyProperty = "foo", MyProperty2 = 15 },
            DataContentType = "application/json",
            Type = "demo.event"
        };


        var delivered = await myProducer.ProduceAsync("flow-topic", cloudEvent);
        log.LogInformation(delivered);
    }
}

internal class MyProducer(IMessageProducer<CloudEvent> producer) : IMyProducer
{
    public async Task<string> ProduceAsync(string topic, CloudEvent cloudEvent)
    {
        if (cloudEvent.DataContentType != "application/json")
        {
            throw new InvalidDataException("expecting application/json data in cloudevent");
        }
        var kafkaMessage = cloudEvent.ToKafkaMessage(ContentMode.Binary, new JsonEventFormatter());
        var deliveryReport = await producer.ProduceAsync(topic, kafkaMessage.Key, cloudEvent.Data, new MessageHeaders(kafkaMessage.Headers));
        return $"{deliveryReport.Topic}:{deliveryReport.Partition}:{deliveryReport.Offset}";
    }
}
