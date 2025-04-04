using KafkaFlow;
using Microsoft.Extensions.Logging;

namespace KafkaFlow_demo.Extensions
{
    public class ProducerMiddleware(ILogger<ProducerMiddleware> log) : IMessageMiddleware
    {
        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            log.LogInformation(context.Message.Value.GetType().Name);
            return Task.CompletedTask;
        }
    }
}