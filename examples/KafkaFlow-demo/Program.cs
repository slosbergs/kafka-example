using CloudNative.CloudEvents;
using Confluent.Kafka;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Confluent.Kafka.ProducerConfig producerConfig = new Confluent.Kafka.ProducerConfig()
        {
            Acks = Confluent.Kafka.Acks.All
        };
        var bootstrapHost = "192.168.101.3:9092";
        var schemaRegistryHost = "192.168.101.3:9092";

        services.AddSingleton<MyCloudEventProducer>();

        services.AddKafkaFlowHostedService(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { bootstrapHost })
                .WithSchemaRegistry(config => config.Url = schemaRegistryHost)
                .AddProducer("cloudEventProducer",
                        producer =>
                            producer.WithProducerConfig(producerConfig)
                        //.AddMiddlewares(m =>
                        //    m.AddSingleTypeSerializer<CloudEventSerializer>(typeof(CloudEvent))
                        //)
                        //.AddMiddlewares(middlewares =>
                        //        middlewares
                        //            .AddCloudEventSrAvroSerializer(new AvroSerializerConfig
                        //            { SubjectNameStrategy = SubjectNameStrategy.TopicRecord })
                        //        )
                        )
                .AddConsumer(consumer => consumer
                    .Topic("topic-name")
                    .WithGroupId("sample-group")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .Add<ReconstructToCloudEvent>(MiddlewareLifetime.Singleton)
                        .AddTypedHandlers(h => h.AddHandler<CloudEventHandler>())
                    )
                )
            )
        );
    })
    .Build()
    .RunAsync();