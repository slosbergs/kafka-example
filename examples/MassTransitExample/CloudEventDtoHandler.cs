using Confluent.Kafka;
using MassTransit;
using MassTransitExample.SerDes;
using System.Text.Json;

public class CloudEventDtoHandler(ILogger<CloudEventDtoHandler> log) : IConsumer<CloudEventDto>
{
    public Task Consume(ConsumeContext<CloudEventDto> context)
    {
        var receivedEvent = context.Message;
        log.LogInformation("Received event: {json}", JsonSerializer.Serialize(receivedEvent));
        return Task.CompletedTask;
    }
}