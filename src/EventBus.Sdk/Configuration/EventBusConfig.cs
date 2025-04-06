using Confluent.Kafka;
using System.Collections;

namespace EventBus.Sdk.Configuration;

public class EventBusConfig : IEnumerable<KeyValuePair<string, string>>, IEnumerable
{
    private Dictionary<string, string> _configDictionary;

    public EventBusConfig()
    {
        _configDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public EventBusConfig(IDictionary<string, string> config) : this()
    {
        foreach (var kvp in config)
        {
            _configDictionary[kvp.Key] = kvp.Value;
        }
    }

    public void Add(string key, string value)
    {
        _configDictionary[key] = value;
    }

    public string this[string key]
    {
        get => _configDictionary.TryGetValue(key, out var value) ? value : null;
        set => _configDictionary[key] = value;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _configDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ProducerConfig ProducerConfig => (ProducerConfig)this;
    public ConsumerConfig ConsumerConfig => (ConsumerConfig)this;

    // Explicit cast to ProducerConfig
    public static explicit operator ProducerConfig(EventBusConfig config)
    {
        var producerConfig = new ProducerConfig(config._configDictionary);
        return producerConfig;
    }

    // Explicit cast to ConsumerConfig
    public static explicit operator ConsumerConfig(EventBusConfig config)
    {
        var consumerConfig = new ConsumerConfig(config._configDictionary);
        return consumerConfig;
    }
}
