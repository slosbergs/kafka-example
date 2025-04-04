//using CloudNative.CloudEvents;
//using Confluent.Kafka;
//using Confluent.SchemaRegistry;

//internal class CloudEventAvroSerializer : IAsyncSerializer<CloudEvent>
//{
//    private ISchemaRegistryClient schemaRegistryClient;

//    public CloudEventAvroSerializer(ISchemaRegistryClient schemaRegistryClient)
//    {
//        this.schemaRegistryClient = schemaRegistryClient;
//    }

//    public Task<byte[]> SerializeAsync(CloudEvent data, SerializationContext context)
//    {

//        try
//        {
//            var avroFormatter = new CloudNative.CloudEvents.Avro.AvroEventFormatter();
//            var avroData = avroFormatter.EncodeStructuredModeMessage(data, out var contentType);

//            // fixme, add schema registry validation

//            return Task.FromResult(avroData.ToArray());
//        }
//        catch (AggregateException ex)
//        {
//            throw ex.InnerException;
//        }
//    }
//}