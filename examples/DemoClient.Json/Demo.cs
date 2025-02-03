using CloudNative.CloudEvents;
using EventBus.Sdk.Producer;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AvroBlogExample;

public class Demo(ILogger<Demo> logger, IEventProducer eventProducer)
{
    public async Task Publish()
    {
        var msg = new CloudEvent()
        {
            Data = new FooClass() { Name = "Frank Drebin", FavoriteColor = "RED" },
            DataContentType = "application/json",
            Type = nameof(FooClass),
            Id = Guid.NewGuid().ToString(),
            Time = DateTime.Now,
            Source = new Uri("test", UriKind.Relative)
        };

        var deliveryReport = await eventProducer.ProduceAsync(msg);
        logger.LogInformation("Producerd event: topic {0}, partition {1}, offset {2}", 
            deliveryReport.Topic, deliveryReport.Partition, deliveryReport.Offset);
    }
}

internal class FooClass
{
    public string Name { get; set; }
    public string FavoriteColor { get; set; }
}