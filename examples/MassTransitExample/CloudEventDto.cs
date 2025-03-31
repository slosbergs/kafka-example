// ref https://github.com/MassTransit/MassTransit/blob/5dffe5664b81e6738f5d9411cca7ba178bb0ca9e/src/Transports/MassTransit.KafkaIntegration/KafkaIntegration/Serializers/MassTransitJsonDeserializer.cs


using Avro;
using Avro.Specific;
using System;

namespace MassTransitExample.SerDes;

/// <summary>
/// DTO structure transferred over kafka, this will be transformed into CloudEvent
/// ClearTextData - serialized json data
/// </summary>
public class CloudEventDto : ISpecificRecord
{
    public static readonly Schema _SCHEMA = Schema.Parse(@"
{
    ""type"": ""record"",
    ""name"": ""CloudEventDto"",
    ""fields"": [
        { ""name"": ""Specversion"", ""type"": ""string"", ""default"": ""1.0"" },
        { ""name"": ""Type"", ""type"": [""null"", ""string""], ""default"": null },
        { ""name"": ""Source"", ""type"": [""null"", ""string""], ""default"": null },
        { ""name"": ""Id"", ""type"": [""null"", ""string""], ""default"": null },
        { ""name"": ""Time"", ""type"": [""null"", ""string""], ""default"": null },
        { ""name"": ""CorrelationId"", ""type"": [""null"", ""string""], ""default"": null },
        { ""name"": ""DataContentType"", ""type"": [""null"", ""string""], ""default"": ""application/json"" },
        { ""name"": ""ClearTextData"", ""type"": [""null"", ""string""], ""default"": null }
    ]
}");

    public Schema Schema => _SCHEMA;

    public string Specversion { get; } = "1.0";
    public required string Type { get; set; }
    public required string Source { get; set; }
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required DateTime Time { get; set; }
    public required string CorrelationId { get; set; }
    public string DataContentType { get; set; } = "application/json";
    public required string ClearTextData { get; set; }

    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => Specversion,
            1 => Type,
            2 => Source,
            3 => Id,
            4 => Time.ToString("o"), // ISO 8601 format
            5 => CorrelationId,
            6 => DataContentType,
            7 => ClearTextData,
            _ => throw new AvroRuntimeException($"Invalid field index: {fieldPos}")
        };
    }

    public void Put(int fieldPos, object value)
    {
        switch (fieldPos)
        {
            case 1: Type = (string)value; break;
            case 2: Source = (string)value; break;
            case 3: Id = (string)value; break;
            case 4: Time = DateTime.Parse((string)value); break;
            case 5: CorrelationId = (string)value; break;
            case 6: DataContentType = (string)value; break;
            case 7: ClearTextData = (string)value; break;
            default: throw new AvroRuntimeException($"Invalid field index: {fieldPos}");
        }
    }
}
