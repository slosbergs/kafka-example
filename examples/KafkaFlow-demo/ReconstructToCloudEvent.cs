using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using KafkaFlow;

internal class ReconstructToCloudEvent : IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        Message<string?, byte[]> foo = new()
        {
            Value = (byte[])context.Message.Value,
            Headers = (Headers)context.Headers,
            Key = (string?)context.Message.Key
        };

        var cloudEvent = foo.ToCloudEvent(new JsonEventFormatter());
        context.SetMessage(cloudEvent.GetPartitionKey(), cloudEvent);

        await next(context).ConfigureAwait(false);
    }
}