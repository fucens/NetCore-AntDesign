using Domain.Example.Commands;
using Domain.Example.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Example.Writer
{
    public class ExampleRepository : IExampleRepository
    {
        private readonly AppDbContext _context;

        public ExampleRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(CreateCommand Cmd)
        {
            //_context.Examples.Add(new ExampleEntity
            //{
            //    BoolField = Cmd.BoolField,
            //    BoolFieldNull = Cmd.BoolFieldNull,
            //    ByteField = Cmd.ByteField,
            //    ByteFieldNull = Cmd.ByteFieldNull,
            //    CharField = Cmd.CharField,
            //    CharFieldNull = Cmd.CharFieldNull,
            //    DecimalField = Cmd.DecimalField,
            //    DecimalFieldNull = Cmd.DecimalFieldNull,
            //    DoubleField = Cmd.DoubleField,
            //    DoubleFieldNull = Cmd.DoubleFieldNull,
            //    EnumField = Cmd.EnumField,
            //    EnumFieldNull = Cmd.EnumFieldNull,
            //    FloatField = Cmd.FloatField,
            //    FloatFieldNull = Cmd.FloatFieldNull,
            //    IntField = Cmd.IntField,
            //    IntFieldNull = Cmd.IntFieldNull,
            //    LongField = Cmd.LongField,
            //    LongFieldNull = Cmd.LongFieldNull,
            //    SbyteField = Cmd.SbyteField,
            //    SbyteFieldNull = Cmd.SbyteFieldNull,
            //    ShortField = Cmd.ShortField,
            //    ShortFieldNull = Cmd.ShortFieldNull,
            //    StringFiled = Cmd.StringFiled,
            //    StructField = Cmd.StructField,
            //    StructFieldNull = Cmd.StructFieldNull,
            //    UintField = Cmd.UintField,
            //    UintFieldNull = Cmd.UintFieldNull,
            //    UlongField = Cmd.UlongField,
            //    UlongFieldNull = Cmd.UlongFieldNull,
            //    UshortField = Cmd.UshortField,
            //    UshortFieldNull = Cmd.UshortFieldNull
            //});
            _context.SaveChanges();
        }

        public async Task CreateAsync(CreateCommand Cmd)
        {
            await Task.Run(() => this.Create(Cmd));
        }

    }
}
