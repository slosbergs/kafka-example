using CloudNative.CloudEvents;

internal interface IMyProducer
{
    Task<string> ProduceAsync(string topic, CloudEvent cloudEvent);
}