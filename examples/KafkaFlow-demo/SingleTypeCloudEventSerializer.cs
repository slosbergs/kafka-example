using CloudNative.CloudEvents;
using Confluent.Kafka;
using KafkaFlow;

public class SingleTypeCloudEventSerializer : KafkaFlow.ISerializer
{
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        throw new NotImplementedException();
    }
}