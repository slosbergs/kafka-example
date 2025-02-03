using Confluent.Kafka;

namespace EventBus.Sdk.Configuration;
public class EventBusConfig
{
    public SecurityConfig Security { get; set; }
    public EvbProducerConfig KafkaProducer { get; set; }
    public EvbConsumerConfig KafkaConsumer { get; set; }
    public string BootstrapServers { get; set; }
}

public class EvbConsumerConfig : ConsumerConfig
{
    public string? Topics { get; set; }

    public new AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;
    public new int? QueuedMinMessages { get; set; } = 10;

    public new int? SessionTimeoutMs = (int)TimeSpan.FromMinutes(3).TotalMilliseconds;

    public new bool EnableAutoCommit { get; } = false;
}

public class SecurityConfig
{
    public string SslCertificate { get; set; }
    public string SslKey { get; set; }
    public bool EnableAuthentication { get; set; }
    public SaslMechanism? SaslMechanism { get; set; }
    public SecurityProtocol? SecurityProtocol { get; set; }
}

public class EvbProducerConfig : ProducerConfig
{
    public string Topic { get; set; }
    public new Acks Acks { get; set; } = Acks.All;
    public new bool EnableIdempotence { get; set; } = true;
    public new double LingerMs { get; set; } = 0;
    public new int? MessageSendMaxRetries { get; set; } = 5;

    public new string DeliveryReportFields { get; set; } = "status";
}


