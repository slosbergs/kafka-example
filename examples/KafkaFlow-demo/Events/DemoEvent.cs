using Avro;
using Avro.Specific;

public class DemoEvent : ISpecificRecord
{
    public static Schema _SCHEMA = Schema.Parse(@"{
        ""type"": ""record"",
        ""name"": ""DemoEvent"",
        ""namespace"": ""io.demo"",
        ""fields"": [
            { ""name"": ""Id"", ""type"": ""int"" },
            { ""name"": ""MyProperty"", ""type"": [""null"", ""string""], ""default"": null },
            { ""name"": ""MyProperty2"", ""type"": ""double"" }
        ]
    }");

    public virtual Schema Schema => _SCHEMA;

    public int Id { get; set; }
    public string? MyProperty { get; set; }
    public double MyProperty2 { get; set; }

    public object Get(int fieldPos) => fieldPos switch
    {
        0 => Id,
        1 => MyProperty,
        2 => MyProperty2,
        _ => throw new AvroRuntimeException("Bad index " + fieldPos)
    };

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                Id = (int)fieldValue;
                break;
            case 1:
                MyProperty = fieldValue as string;
                break;
            case 2:
                MyProperty2 = (double)fieldValue;
                break;
            default:
                throw new AvroRuntimeException("Bad index " + fieldPos);
        }
    }
}
