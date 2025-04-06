using CloudNative.CloudEvents;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using EventBus.Sdk.Configuration;
using EventBus.Sdk.Producer;
using KafkaFlow;
using KafkaFlow_demo.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        var bootstrapHost = "192.168.101.3:9092";
        var schemaRegistryHost = "192.168.101.3:9092";


        Confluent.Kafka.ProducerConfig producerConfig = new Confluent.Kafka.ProducerConfig()
        {
            EnableDeliveryReports = true,
            Acks = Confluent.Kafka.Acks.All,
            EnableSslCertificateVerification = false,
            SecurityProtocol = SecurityProtocol.Plaintext,

        };

        services.Configure<EventBusConfig>(options =>
        {
            options.Add("bootstrap.servers", bootstrapHost);
            options.Add("enable.idempotence", "true");

            options["security.protocol"] = "PLAINTEXT";  // This is the key setting to disable SSL
            options["group.id"] = "my-consumer-group";
            options["enable.ssl.certificate.verification"] = "false";
            options["compression.type"] = "gzip";
            options["debug"] = "msg";


        });


        services.AddSingleton<MyCloudEventProducer>();

        //services.AddTransient<IMyProducer, MyProducer>();
        services.AddTransient<IEventProducer, KafkaProducer>();
        services.AddTransient<ISchemaRegistryClient, CachedSchemaRegistryClient>(sp =>
            new CachedSchemaRegistryClient([new("schema.registry.url", schemaRegistryHost)]));

        services.AddHostedService<TestWorker>();

        services.AddKafkaFlowHostedService(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers(new[] { bootstrapHost })
                //.WithSchemaRegistry(config => config.Url = schemaRegistryHost)
                .WithSecurityInformation(security =>
                {
                    security.SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol.Plaintext;
                    security.SaslMechanism = KafkaFlow.Configuration.SaslMechanism.Plain;
                    security.EnableSslCertificateVerification = false;
                })
                .AddProducer<CloudEvent>(
                        producer =>
                            producer.WithProducerConfig(producerConfig)

                        .AddMiddlewares(m => m
                            .Add<ProducerMiddleware>()

                            //.AddSchemaRegistryAvroSerializer()
                            //.Add<ProducerMiddleware>()

                            )
                        //    m.AddSingleTypeSerializer<CloudEventSerializer>(typeof(CloudEvent))
                        //)
                        //.AddMiddlewares(middlewares =>
                        //        middlewares
                        //            .AddCloudEventSrAvroSerializer(new AvroSerializerConfig
                        //            { SubjectNameStrategy = SubjectNameStrategy.TopicRecord })
                        //        )
                        )
            //.AddConsumer(consumer => consumer
            //    .Topic("topic-name")
            //    .WithGroupId("sample-group")
            //    .WithBufferSize(100)
            //    .WithWorkersCount(10)
            //    .AddMiddlewares(middlewares => middlewares
            //        .Add<ReconstructToCloudEvent>(MiddlewareLifetime.Singleton)
            //        .AddTypedHandlers(h => h.AddHandler<CloudEventHandler>())
            //    )
            //)
            )
        );
    })
    .Build()
    .RunAsync();
