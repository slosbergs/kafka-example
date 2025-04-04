using Avro;
using Avro.Specific;
using System;
using System.Text.Json.Serialization;
namespace MassTransitExample;

public class KafkaJsonMessage
{
    public string Schema { get; set; }
    public int Payload { get; set; }
}



public class KafkaJsonMessageAvro : ISpecificRecord
{
    private const string schemaString = @"{ ""type"": ""record"",
            ""name"": ""KafkaJsonMessage"",
            ""fields"": [
                { ""name"": ""Schema"", ""type"": ""string"" },
                { ""name"": ""Payload"", ""type"": ""int"" }
            ]
        }";
    public static Schema _SCHEMA = Schema.Parse(schemaString);

    public virtual Schema Schema => _SCHEMA;
    public string SchemaField { get; set; }
    public int Payload { get; set; }

    public object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => SchemaField,
            1 => Payload,
            _ => throw new AvroRuntimeException("Bad index " + fieldPos)
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                SchemaField = (string)fieldValue;
                break;
            case 1:
                Payload = (int)fieldValue;
                break;
            default:
                throw new AvroRuntimeException("Bad index " + fieldPos);
        }
    }
}