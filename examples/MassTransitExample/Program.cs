using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.KafkaIntegration.Caching;
using MassTransitExample;

var builder = Host.CreateApplicationBuilder(args);


var kafkaConsumerConfig = new ConsumerConfig()
{
    Acks = Acks.All,
    GroupId = "demo-consumer",
    AutoOffsetReset = AutoOffsetReset.Earliest
};


builder.Services.AddMassTransit(mt =>
    {
        mt.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

        mt.AddRider(rider =>
        {
            rider.AddProducer<KafkaJsonMessage>("demo-topic", (context, cfg) =>
            {
                //cfg.SetValueSerializer(new CloudEventSerializer());
                // Configure the AVRO serializer, with the schema registry client
                //cfg.SetValueSerializer(new AvroSerializer<KafkaJsonMessage>(context.GetRequiredService<ICachedTopicProducer>()));
            });

            rider.AddConsumer<KafkaMessageConsumer>((context, cfg) =>
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

                k.TopicEndpoint<KafkaJsonMessage>("demo-topic", kafkaConsumerConfig, e =>
                {
                    // ref https://masstransit.io/documentation/configuration/transports/kafka#scalability
                    e.ConcurrentConsumerLimit = 100;
                    e.ConcurrentMessageLimit = 100;
                    e.MessageLimit = 100;
                    e.PrefetchCount = 500;

                    e.ConfigureConsumer<KafkaMessageConsumer>(context);
                });
            });
        });
    });


builder.Services.AddSingleton<Counter>();
//builder.Services.AddHostedService<Worker>();
var host = builder.Build();


host.Run();
