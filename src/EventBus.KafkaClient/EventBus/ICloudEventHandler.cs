using CloudNative.CloudEvents;
using System.Threading.Tasks;

namespace AvroBlogExamples.EventBus;

public interface ICloudEventHandler
{
    Task HandleMessageAsync(CloudEvent message);
}
