using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Example.Entity
{
    public enum ExampleEnum
    {
        Item1,
        Item2,
        Item3
    }

    public struct ExampleStruct
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
    }

    public class ExampleEntity : IAudit<Guid>
    {
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

        public ExampleEnum EnumField { get; set; }
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

        [NotMapped]
        public ExampleStruct StructField { get; set; }
        [NotMapped]
        public ExampleStruct? StructFieldNull { get; set; }

        public uint UintField { get; set; }
        public uint? UintFieldNull { get; set; }

        public ulong UlongField { get; set; }
        public ulong? UlongFieldNull { get; set; }

        public ushort UshortField { get; set; }
        public ushort? UshortFieldNull { get; set; }

        public string StringFiled { get; set; }

        public ICollection<ExampleDetail> Detail { get; set; }


        public void AddDetail(ExampleDetail detail)
        {

        }
    }

    public class ExampleEntityConfiguration : IEntityTypeConfiguration<ExampleEntity>
    {
        public void Configure(EntityTypeBuilder<ExampleEntity> builder)
        {
            builder.Property(p => p.StringFiled).HasMaxLength(20);
            builder.Property(p => p.IntField).IsRequired();
            builder.Property(p => p.DecimalField).Metadata.Relational().ColumnType = "decimal(18,8)";
            builder.Property(p => p.DecimalFieldNull).Metadata.Relational().ColumnType = "decimal(18,8)";
            builder.HasQueryFilter(p => !p.IsDelete);
            builder.HasIndex(p => p.StringFiled).ForMySqlIsFullText(true);
        }
    }


}
