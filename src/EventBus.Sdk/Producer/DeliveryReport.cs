using Confluent.Kafka;
using System;

namespace EventBus.Sdk.Producer;

public record DeliveryReport
{
    public string? Topic { get; private set; }
    public int Partition { get; private set; }
    public long Offset { get; private set; }

    internal static DeliveryReport From(TopicPartitionOffset topicPartitionOffset)
    {
        return new()
        {
            Topic = topicPartitionOffset.Topic,
            Partition = topicPartitionOffset.Partition,
            Offset = topicPartitionOffset.Offset
        };
    }
}