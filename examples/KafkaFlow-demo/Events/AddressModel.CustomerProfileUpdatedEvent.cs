using Avro;
using Avro.Specific;
using Confluent.SchemaRegistry;

namespace io.cloudevents
{
    public static class Enums
    {
        public enum ProfileType { Retail, Commercial }
    }

    public class AddressModel : ISpecificRecord
    {
        public static Avro.Schema _SCHEMA = Avro.Schema.Parse(@"{
            'type': 'record',
            'name': 'AddressModel',
            'fields': [
                { 'name': 'streetLine1', 'type': ['null', 'string'], 'default': null },
                { 'name': 'streetLine2', 'type': ['null', 'string'], 'default': null },
                { 'name': 'streetLine3', 'type': ['null', 'string'], 'default': null },
                { 'name': 'city', 'type': ['null', 'string'], 'default': null },
                { 'name': 'zip', 'type': ['null', 'string'], 'default': null },
                { 'name': 'state', 'type': ['null', 'string'], 'default': null },
                { 'name': 'zipPlusFour', 'type': ['null', 'string'], 'default': null }
            ]
        }".Replace("'", "\""));

        public virtual Avro.Schema Schema => _SCHEMA;
        public string? StreetLine1 { get; set; }
        public string? StreetLine2 { get; set; }
        public string? StreetLine3 { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public string? State { get; set; }
        public string? ZipPlusFour { get; set; }

        public object Get(int fieldPos) => fieldPos switch
        {
            0 => StreetLine1,
            1 => StreetLine2,
            2 => StreetLine3,
            3 => City,
            4 => Zip,
            5 => State,
            6 => ZipPlusFour,
            _ => throw new AvroRuntimeException("Bad index " + fieldPos)
        };

        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: StreetLine1 = (string?)fieldValue; break;
                case 1: StreetLine2 = (string?)fieldValue; break;
                case 2: StreetLine3 = (string?)fieldValue; break;
                case 3: City = (string?)fieldValue; break;
                case 4: Zip = (string?)fieldValue; break;
                case 5: State = (string?)fieldValue; break;
                case 6: ZipPlusFour = (string?)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos);
            }
        }


        public class RetailCustomerProfileDelta : ISpecificRecord
        {
            public static Avro.Schema _SCHEMA = Avro.Schema.Parse(@"{
            'type': 'record',
            'name': 'RetailCustomerProfileDelta',
            'fields': [
                { 'name': 'firstName', 'type': ['null', 'string'], 'default': null },
                { 'name': 'middleName', 'type': ['null', 'string'], 'default': null },
                { 'name': 'lastName', 'type': ['null', 'string'], 'default': null },
                { 'name': 'nameSuffix', 'type': ['null', 'string'], 'default': null },
                { 'name': 'address', 'type': ['null', 'io.cloudevents.AddressModel'], 'default': null }
                ]
        }".Replace("'", "\""));

            public Avro.Schema Schema => _SCHEMA;
            public string? FirstName { get; set; }
            public string? MiddleName { get; set; }
            public string? LastName { get; set; }
            public string? NameSuffix { get; set; }
            public AddressModel? Address { get; set; }

            public object Get(int fieldPos) => fieldPos switch
            {
                0 => FirstName,
                1 => MiddleName,
                2 => LastName,
                3 => NameSuffix,
                4 => Address,
                _ => throw new AvroRuntimeException("Bad index " + fieldPos)
            };

            public void Put(int fieldPos, object fieldValue)
            {
                switch (fieldPos)
                {
                    case 0: FirstName = (string?)fieldValue; break;
                    case 1: MiddleName = (string?)fieldValue; break;
                    case 2: LastName = (string?)fieldValue; break;
                    case 3: NameSuffix = (string?)fieldValue; break;
                    case 4: Address = (AddressModel?)fieldValue; break;
                    default: throw new AvroRuntimeException("Bad index " + fieldPos);
                }
            }
        }

        public class CommercialCustomerProfileDelta : ISpecificRecord
        {
            public static Avro.Schema _SCHEMA = Avro.Schema.Parse(@"{
            'type': 'record',
            'name': 'CommercialCustomerProfileDelta',
            'fields': [
                { 'name': 'customerName', 'type': ['null', 'string'], 'default': null },
                { 'name': 'customerNameAdditional', 'type': ['null', 'string'], 'default': null },
                { 'name': 'address', 'type': ['null', 'io.cloudevents.AddressModel'], 'default': null }
            ]
        }".Replace("'", "\""));

            public virtual Avro.Schema Schema => _SCHEMA;
            public string? CustomerName { get; set; }
            public string? CustomerNameAdditional { get; set; }
            public AddressModel? Address { get; set; }

            public object Get(int fieldPos) => fieldPos switch
            {
                0 => CustomerName,
                1 => CustomerNameAdditional,
                2 => Address,
                _ => throw new AvroRuntimeException("Bad index " + fieldPos)
            };

            public void Put(int fieldPos, object fieldValue)
            {
                switch (fieldPos)
                {
                    case 0: CustomerName = (string?)fieldValue; break;
                    case 1: CustomerNameAdditional = (string?)fieldValue; break;
                    case 2: Address = (AddressModel?)fieldValue; break;
                    default: throw new AvroRuntimeException("Bad index " + fieldPos);
                }
            }
        }

        public class CustomerProfileUpdatedEvent : ISpecificRecord
        {
            public static Avro.Schema _SCHEMA = Avro.Schema.Parse(@"{
            'type': 'record',
            'name': 'CustomerProfileUpdatedEvent',
            'fields': [
                { 'name': 'customerId', 'type': 'string' },
                { 'name': 'profileType', 'type': { 'type': 'enum', 'name': 'ProfileType', 'symbols': ['Retail', 'Commercial'] } },
                { 'name': 'profile', 'type': ['io.cloudevents.RetailCustomerProfileDelta', 'io.cloudevents.CommercialCustomerProfileDelta'] }
            ]
        }".Replace("'", "\""));

            public virtual Avro.Schema Schema => _SCHEMA;
            public string CustomerId { get; set; } = string.Empty;
            public string ProfileType { get; set; } = string.Empty;
            public ISpecificRecord Profile { get; set; } = null!;

            public object Get(int fieldPos) => fieldPos switch
            {
                0 => CustomerId,
                1 => ProfileType,
                2 => Profile,
                _ => throw new AvroRuntimeException("Bad index " + fieldPos)
            };

            public void Put(int fieldPos, object fieldValue)
            {
                switch (fieldPos)
                {
                    case 0: CustomerId = (string)fieldValue; break;
                    case 1: ProfileType = (string)fieldValue; break;
                    case 2: Profile = (ISpecificRecord)fieldValue; break;
                    default: throw new AvroRuntimeException("Bad index " + fieldPos);
                }
            }
        }
    }
}
