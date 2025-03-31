using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using MassTransitExample;
using MassTransitExample.SerDes;

var builder = Host.CreateApplicationBuilder(args);

string Topic = "demo-topic";
string GroupId = "demo-consumer";

var kafkaConsumerConfig = new ConsumerConfig()
{
    Acks = Acks.All,
    GroupId = GroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

builder.Services.AddSingleton<ISchemaRegistryClient>(
    new CachedSchemaRegistryClient(new Dictionary<string, string>
    {
        {"schema.registry.url", "192.168.101.3:8081"},
    }));



builder.Services.AddMassTransit(mt =>
    {
        mt.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));


        mt.AddRider(rider =>
        {
            rider.AddProducer<string, CloudEventDto>(Topic, (context, cfg) =>
            {
                cfg.EnableDeliveryReports = true;
                cfg.EnableIdempotence = true;
                // Configure the AVRO serializer, with the schema registry client
                cfg.SetValueSerializer(new AvroSerializer<CloudEventDto>(context.GetRequiredService<ISchemaRegistryClient>()).AsSyncOverAsync());
            });

            rider.AddConsumer<CloudEventDtoHandler>((context, cfg) =>
            {
                // Configure the AVRO serializer, with the schema registry client
                //cfg.SetValueSerializer(new AvroSerializer<KafkaJsonMessage>(context.GetRequiredService<ICachedTopicProducer>()));
            });

            rider.UsingKafka((context, k) =>
            {
                k.SecurityProtocol = SecurityProtocol.Plaintext;
                k.Host("192.168.101.3:9092", host =>
                {
                    //host.UseSsl(c =>
                    //{
                    //    c.SslCaPem = "cacert.pem";
                    //});
                    //host.UseSasl(s =>
                    //{
                    //    s.OauthbearerConfig = "space-separated name-value pairs";
                    //    s.Mechanism = SaslMechanism.OAuthBearer;
                    //});
                });

                k.TopicEndpoint<string, CloudEventDto>(Topic, GroupId, e =>
                {
                    e.AutoOffsetReset = AutoOffsetReset.Earliest;

                    e.SetValueDeserializer(new AvroDeserializer<CloudEventDto>(context.GetRequiredService<ISchemaRegistryClient>()).AsSyncOverAsync());

                    // the number of concurrent messages, per partition
                    e.ConcurrentMessageLimit = 20;

                    // create up to two Confluent Kafka consumers, increases throughput with multiple partitions
                    e.ConcurrentConsumerLimit = 10;

                    // delivery only one message per key value within a partition at a time (default)
                    e.ConcurrentDeliveryLimit = 1;

                    // Adding this filter allows AVRO union messages to be consumed directly
                    // ref 
                    //e.UseAvroUnionMessageTypeFilter<CloudEventDto>(m => m.Event);

                    e.ConfigureConsumer<CloudEventDtoHandler>(context);
                });
            });
        });
    });


builder.Services.AddSingleton<Counter>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();


host.Run();
