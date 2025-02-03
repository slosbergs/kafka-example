using AvroBlogExamples.EventBus;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
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
        logger.LogInformation("Producerd event, {@deliveryReport}", deliveryReport);
    }
}

internal class FooClass
{
    public string Name { get; set; }
    public string FavoriteColor { get; set; }
}