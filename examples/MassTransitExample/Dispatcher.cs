using MassTransit;
using MassTransit.Middleware;
using System.Diagnostics.CodeAnalysis;

internal class Dispatcher : PipeContext
{
    public CancellationToken CancellationToken => throw new NotImplementedException();

    public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory) where T : class
    {
        throw new NotImplementedException();
    }

    public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory) where T : class
    {
        throw new NotImplementedException();
    }

    public bool HasPayloadType(Type payloadType)
    {
        throw new NotImplementedException();
    }

    public bool TryGetPayload<T>([NotNullWhen(true)] out T? payload) where T : class
    {
        throw new NotImplementedException();
    }
}