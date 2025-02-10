using Avro.Generic;
using CloudNative.CloudEvents;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using KafkaFlow;

internal class CloudEventHandler : IMessageHandler<CloudEvent>
{
    /// <summary>
    /// this is where we can do actual deserialziation
    /// </summary>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Handle(IMessageContext context, CloudEvent cloudEvent)
    {
        using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
        var valueDeserializer = new AvroDeserializer<GenericRecord>(schemaRegistry);
        

            var eventData = valueDeserializer.DeserializeAsync(
                                    cloudEvent.Data == null ? ReadOnlyMemory<byte>.Empty : (byte[])cloudEvent.Data,
                                    cloudEvent.Data == null,
                                    new SerializationContext(
                                        MessageComponentType.Value,
        topic,
                                        consumeResult.Message.Headers)).Result;

    }
}