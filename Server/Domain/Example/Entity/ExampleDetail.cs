using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Example.Entity
{
    public class ExampleDetail : IAudit<Guid>
    {
        public string StringFiled { get; set; }
        public int IntField { get; set; }
        public int? IntFieldNull { get; set; }
    }

    public class ExampleDetailConfiguration : IEntityTypeConfiguration<ExampleDetail>
    {
        public void Configure(EntityTypeBuilder<ExampleDetail> builder)
        {
            builder.Property(p => p.StringFiled).HasMaxLength(20);
            builder.Property(p => p.IntField).IsRequired();
            builder.HasQueryFilter(p => !p.IsDelete);
            builder.HasIndex(p => p.StringFiled).ForMySqlIsFullText(true);
        }
    }
}
