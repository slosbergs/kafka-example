using Confluent.Kafka;
using MassTransit;
using MassTransit.Monitoring.Performance;
using MassTransitExample;
using System.Text.Json;
using System.Threading;

public class KafkaMessageConsumer(ILogger<KafkaMessageConsumer> log, Counter counter) : IConsumer<KafkaJsonMessage>
{

    public async Task Consume(ConsumeContext<KafkaJsonMessage> context)
    {

        var json = JsonSerializer.Serialize(context.Message);
        log.LogInformation("{event}", json);

        await Task.Delay(500);
        await counter.Tick();

        if (json.Contains("error"))
            throw new Exception("test exception");


        //return Task.CompletedTask;

    }
}

public class Counter (ILogger<Counter> log)
{
    private SemaphoreSlim _semaphore { get; set; } = new SemaphoreSlim(1);
    private DateTime? Start { get; set; }
    private DateTime? End { get; set; }

    public TimeSpan GetTimeSpan => End.Value - Start.Value;
    public async ValueTask Tick()
    {
        await _semaphore.WaitAsync(); // Async wait for semaphore
        if (!Start.HasValue)
        {
            Start = DateTime.UtcNow;
        }
        End = DateTime.UtcNow;
        

        log.LogInformation("milliseconds passed: {ms}", GetTimeSpan.TotalMilliseconds );
        _semaphore.Release();
    }
}