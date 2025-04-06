using Avro.Specific;
using CloudNative.CloudEvents;

internal interface IMyProducer
{
    Task<string> ProduceAsync<T>(string topic, CloudEvent cloudEvent);
}