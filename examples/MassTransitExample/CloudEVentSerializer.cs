using CloudNative.CloudEvents;
using Confluent.Kafka;
using MassTransit.Serialization;
using MassTransitExample;
using System.Text.Json;

// ref https://github.com/MassTransit/MassTransit/blob/5dffe5664b81e6738f5d9411cca7ba178bb0ca9e/src/Transports/MassTransit.KafkaIntegration/KafkaIntegration/Serializers/MassTransitJsonDeserializer.cs

namespace MassTransitExample.SerDes;

public class MassTransitJsonDeserializer<T> : IDeserializer<CloudEvent>
{
    public CloudEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (data.IsEmpty && isNull)
            return default;

        throw new NotImplementedException();
        //return JsonSerializer.Deserialize<T>(data, SystemTextJsonMessageSerializer.Options);
    }
}
