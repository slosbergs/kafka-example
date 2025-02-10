using CloudNative.CloudEvents;
using EventBus.Sdk.Producer;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoClient.Json;

public class Demo(ILogger<Demo> logger, IEventProducer eventProducer)
{
    
    public async Task Publish<T>(T data)
    {
        var msg = new CloudEvent()
        {
            Data = data,
            DataContentType = "application/json",
            Type = nameof(T),
            Id = Guid.NewGuid().ToString(),
            Time = DateTime.Now,
            Source = new Uri("test", UriKind.Relative)
        };

        var deliveryReport = await eventProducer.ProduceAsync(msg);
        logger.LogInformation("Produced event: id {0}, topic {1}, partition {2}, offset {3}",
            msg.Id, deliveryReport.Topic, deliveryReport.Partition, deliveryReport.Offset);
    }
}

internal class FooClass
{
    public string Name { get; set; }
    public string FavoriteColor { get; set; }
}