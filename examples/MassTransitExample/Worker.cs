using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitExample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider provider;

        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            this.provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = provider.CreateScope())
            {
                var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaJsonMessage>>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    //   _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await producer.Produce(new KafkaJsonMessage() { Payload = 111 }, stoppingToken);

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
