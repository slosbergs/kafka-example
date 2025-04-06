using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;
using CloudNative.CloudEvents.Kafka;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using EventBus.Sdk.Producer;
using KafkaFlow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class TestWorker(ILogger<TestWorker> log, IEventProducer myProducer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        //CloudEvent cloudEvent = new CloudEvent()
        //{
        //    Id = Guid.NewGuid().ToString(),
        //    Source = new Uri("/demo", UriKind.Relative),
        //    Time = DateTime.UtcNow,
        //    Data = new CustomerProfileUpdatedEvent()
        //    {
        //        CustomerId = "1234",
        //        ProfileType = KafkaFlow_demo.Events.Enums.ProfileType.Retail,
        //        Profile = new RetailCustomerProfileDelta() { FirstName = "dick", LastName = "dickinson" }
        //    },
        //    DataContentType = "application/json",
        //    Type = "demo.event"
        //};

        var dataPayload = new DemoEvent()
        {
            CustomerId = "1234",
            ProfileType = KafkaFlow_demo.Events.Enums.ProfileType.Retail,
            //Profile = new RetailCustomerProfileDelta() { FirstName = "dick", LastName = "dickinson" }
        };

        CloudEvent cloudEvent = new CloudEvent()
        {
            Id = Guid.NewGuid().ToString(),
            Source = new Uri("/demo", UriKind.Relative),
            Time = DateTime.UtcNow,
            Data = dataPayload,
            DataContentType = "application/json",
            Type = "demo.event"
        };


        //using (var serStream = new MemoryStream())
        //{
        //    var writer = new GenericDatumWriter<CustomerProfileUpdatedEvent>(((CustomerProfileUpdatedEvent)cloudEvent.Data).Schema);
        //    var encoder = new BinaryEncoder(serStream);
        //    writer.Write((CustomerProfileUpdatedEvent)cloudEvent.Data, encoder);
        //    var rawAvro = serStream.ToArray();
        //    Console.WriteLine($"Manual serialization size: {rawAvro.Length} bytes");
        //}


        var delivered = await myProducer.ProduceAsync<DemoEvent>("topic_60", dataPayload, cloudEvent, "foo", stoppingToken);
        log.LogInformation("delivered to {partition}:{offset}", delivered.Partition, delivered.Offset);
    }
}

internal class MyProducer(IMessageProducer<CloudEvent> producer, ISchemaRegistryClient srClient) : IMyProducer
{
    public async Task<string> ProduceAsync<T>(string topic, CloudEvent cloudEvent)
    {
        if (cloudEvent.DataContentType != "application/json")
        {
            throw new InvalidDataException("expecting application/json data in cloudevent");
        }

        cloudEvent[Partitioning.PartitionKeyAttribute] = cloudEvent.Id;
        var kafkaMessage = cloudEvent.ToKafkaMessage(ContentMode.Binary, new JsonEventFormatter());

        var valueSerializer = GetSerializer((T)cloudEvent.Data, srClient);
        var avroBytes = await valueSerializer.SerializeAsync((T)cloudEvent.Data,
                            new SerializationContext(
                                MessageComponentType.Value,
                                topic));

        var deliveryReport = await producer.ProduceAsync(topic, kafkaMessage.Key, avroBytes, new MessageHeaders(kafkaMessage.Headers));
        return $"{deliveryReport.Topic}:{deliveryReport.Partition}:{deliveryReport.Offset}";
    }

    public AvroSerializer<T> GetSerializer<T>(T message, ISchemaRegistryClient srClient)
    {
        return new AvroSerializer<T>(srClient);
    }
}
