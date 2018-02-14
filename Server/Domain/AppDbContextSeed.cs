using Domain.Example.Commands;
using Domain.Example.Entity;
using Domain.Example.Writer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public interface IAppDbContextSeed
    {
        void Seed();
    }

    public class AppDbContextSeed : IAppDbContextSeed
    {
        private readonly AppDbContext _context;
        private readonly IExampleRepository _exampleRepository;
        private readonly Random _random = new Random();

        public AppDbContextSeed(AppDbContext context, IExampleRepository exampleRepository)
        {
            _context = context;
            _exampleRepository = exampleRepository;
        }

        public void Seed()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            for (int i = 0; i < 27; i++)
            {
                var random = _random.Next(0, 100);

                bool then30 = false;
                if (random < 30)
                    then30 = true;
                else if (random >= 30 && random < 60)
                    then30 = false;

                bool? boolFieldNull = null;
                if (then30)
                    boolFieldNull = true;
               

                byte? byteFieldNull = null;
                if (then30)
                    byteFieldNull = (byte)random;

                char? charFieldNull = null;
                if (then30)
                    charFieldNull = 'B';


                decimal? decimalFieldNull = null;
                if (then30)
                    decimalFieldNull = i;

                double? doubleFieldNull = null;
                if (then30)
                    doubleFieldNull = i;

                ExampleEnum? enumFieldNull = null;
                if (then30)
                    enumFieldNull = ExampleEnum.Item3;

                float? floatFieldNull = null;
                if (then30)
                    floatFieldNull = i;

                int? intFieldNull = null;
                if (then30)
                    intFieldNull = i;

                int? longFieldNull = null;
                if (then30)
                    longFieldNull = i;

                sbyte? sbyteFieldNull = null;
                if (then30)
                    sbyteFieldNull = -7;

                sbyte? shortFieldNull = null;
                if (then30)
                    shortFieldNull = -7;

                uint? uintFieldNull = null;
                if (then30)
                    uintFieldNull = 7;

                ulong? ulongFieldNull = null;
                if (then30)
                    ulongFieldNull = 7;

                ushort? ushortFieldNull = null;
                if (then30)
                    ushortFieldNull = 666;

                _context.Examples.Add(new ExampleEntity
                {
                    BoolField = random < 50,
                    BoolFieldNull = boolFieldNull,
                    ByteField = (byte)random,
                    ByteFieldNull = byteFieldNull,
                    CharField = 'A',
                    CharFieldNull = charFieldNull,
                    DecimalField = i,
                    DecimalFieldNull = decimalFieldNull,
                    DoubleField = i,
                    DoubleFieldNull = doubleFieldNull,
                    EnumField = ExampleEnum.Item2,
                    EnumFieldNull = enumFieldNull,
                    FloatField = i,
                    FloatFieldNull = floatFieldNull,
                    IntField = i,
                    LongField = i,
                    SbyteField = -5,
                    ShortField = -7,
                    StringFiled = "My Is Str" + i,
                    UintField = 888888,
                    UlongField = 88888,
                    UshortField = 6666,
                    IntFieldNull = intFieldNull,
                    LongFieldNull = longFieldNull,
                    SbyteFieldNull = sbyteFieldNull,
                    ShortFieldNull = shortFieldNull,
                    UintFieldNull = uintFieldNull,
                    UlongFieldNull = ulongFieldNull,
                    UshortFieldNull = ushortFieldNull,
                });

            }
            _context.SaveChanges();

        }


    }
}
