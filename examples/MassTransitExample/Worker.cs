using MassTransit;
using MassTransitExample.SerDes;
using System.Text.Json;

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
                //var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaJsonMessage>>(); 
                var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<string, CloudEventDto>>();
                while (!stoppingToken.IsCancellationRequested)
                {

                    var payload = new KafkaJsonMessage() { Payload = DateTime.Now.Second };

                    var msg = new CloudEventDto() { 
                        CorrelationId = Guid.NewGuid().ToString(),
                        Id = Guid.NewGuid().ToString(),
                        Type = "deposits.customer.profile.updated",
                        Time = DateTime.UtcNow,
                        Source = "demo",
                        ClearTextData = JsonSerializer.Serialize(payload) };

                    //   _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await producer.Produce("partitionkey", msg, stoppingToken);

                    _logger.LogInformation("published...");

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
