using Avro;
using Avro.Specific;
using KafkaFlow_demo.Events;

public class DemoEvent : ISpecificRecord
{
    public static Schema _SCHEMA = Schema.Parse(@"{
        'type': 'record',
        'name': 'DemoEvent',
        'namespace': 'io.demo',
        'fields': [
            { 'name': 'Id', 'type': 'int' },
             { 'name': 'customerId', 'type': 'string' }
        ]
    }".Replace("'", "\""));

    public virtual Schema Schema => _SCHEMA;

    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Enums.ProfileType ProfileType { get; set; }

    public object? Get(int fieldPos) => fieldPos switch
    {
        0 => Id,
        1 => CustomerId,
        //2 => ProfileType.ToString(),
        _ => throw new AvroRuntimeException("Bad index " + fieldPos)
    };

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                Id = (int)fieldValue;
                break;
            case 1: CustomerId = (string)fieldValue; break;
            //case 2: ProfileType = Enum.Parse<Enums.ProfileType>((string)fieldValue); break;
            default:
                throw new AvroRuntimeException("Bad index " + fieldPos);
        }
    }
}
