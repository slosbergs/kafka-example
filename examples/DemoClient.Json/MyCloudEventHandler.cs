using CloudNative.CloudEvents;
using EventBus.Sdk.Consumer;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class MyCloudEventHandler : ICloudEventHandler
{
    private readonly ILogger<MyCloudEventHandler> logger;

    public MyCloudEventHandler(ILogger<MyCloudEventHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleMessageAsync(CloudEvent message)
    {
        string json = message.ToString();
        logger.LogInformation("Received message: {json}", json);
        
        return Task.CompletedTask;
    }
}