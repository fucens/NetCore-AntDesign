using Domain.Example.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Example.Results
{
    public class ExampleResult
    {
        public Guid Id { get; set; }

        public bool BoolField { get; set; }
        public bool? BoolFieldNull { get; set; }

        public byte ByteField { get; set; }
        public byte? ByteFieldNull { get; set; }

        public char CharField { get; set; }
        public char? CharFieldNull { get; set; }

        public decimal DecimalField { get; set; }
        public decimal? DecimalFieldNull { get; set; }

        public double DoubleField { get; set; }
        public double? DoubleFieldNull { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ExampleEnum EnumField { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ExampleEnum? EnumFieldNull { get; set; }

        public float FloatField { get; set; }
        public float? FloatFieldNull { get; set; }

        public int IntField { get; set; }
        public int? IntFieldNull { get; set; }

        public long LongField { get; set; }
        public long? LongFieldNull { get; set; }

        public sbyte SbyteField { get; set; }
        public sbyte? SbyteFieldNull { get; set; }

        public short ShortField { get; set; }
        public short? ShortFieldNull { get; set; }

        public ExampleStruct StructField { get; set; }
        public ExampleStruct? StructFieldNull { get; set; }

        public uint UintField { get; set; }
        public uint? UintFieldNull { get; set; }

        public ulong UlongField { get; set; }
        public ulong? UlongFieldNull { get; set; }

        public ushort UshortField { get; set; }
        public ushort? UshortFieldNull { get; set; }

        public string StringFiled { get; set; }

        public DateTime CreateTime { get; set; }
    }


}
