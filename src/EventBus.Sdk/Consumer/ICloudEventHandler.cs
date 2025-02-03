using CloudNative.CloudEvents;
using System.Threading.Tasks;

namespace EventBus.Sdk.Consumer;

public interface ICloudEventHandler
{
    Task HandleMessageAsync(CloudEvent message);
}
