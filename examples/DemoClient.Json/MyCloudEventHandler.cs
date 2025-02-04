using CloudNative.CloudEvents;
using EventBus.Sdk.Consumer;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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
        var payload = (JsonElement)message.Data;
        logger.LogInformation("Received message. Id {id}, {json}", message.Id, payload.ToString());
        
        return Task.CompletedTask;
    }
}