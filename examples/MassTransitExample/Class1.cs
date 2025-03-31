using Confluent.Kafka;
using MassTransitExample.SerDes;

namespace MassTransitExample;

public class KafkaDeliveryMonitor(ILogger<KafkaDeliveryMonitor> log) : IObserver<DeliveryReport<string, CloudEventDto>>
{
    public void OnCompleted()
    {
        // Handle the completion
    }

    public void OnError(Exception error)
    {
        // Handle any error
        log.LogError(error, "Kafka delivery failed");
    }

    public void OnNext(DeliveryReport<string, CloudEventDto> value)
    {
        // Process delivery reports
        if (value.Error != null)
        {
            log.LogError($"Delivery failed: {value.Error.Reason}");
        }
        else
        {
            log.LogInformation("Message {correlationId} delivered to {TopicPartitionOffset}", value.Value.CorrelationId, value.TopicPartitionOffset);
        }
    }

}
