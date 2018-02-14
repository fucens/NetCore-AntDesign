using Domain.Example.Commands;
using Domain.Example.Entity;
using Domain.Example.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Example.Reader
{
    public class ExampleQueries : IExampleQueries
    {
        private readonly AppDbContext _context;

        public ExampleQueries(AppDbContext context)
        {
            _context = context;
        }

        public ExampleResult Get(Guid Id)
        {
            return _context.Examples.Where(x => x.Id == Id).AutoSelect<ExampleEntity, ExampleResult>().Single();
        }


        public PageResult<ExampleResult> GetListTest(PageCommand Cmd)
        {
            var sql = _context.Examples.Select(x => new { x.StringFiled, x.IntField }).ToSql();


            return _context.Examples.ToPage<ExampleEntity, ExampleResult>(Cmd);

        }

        public Task<PageResult<ExampleResult>> GetListAsyncTest(PageCommand Cmd)
        {
            return Task.Run(() => GetListTest(Cmd));
        }








    }
}
