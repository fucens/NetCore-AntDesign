using Domain.Example.Commands;
using Domain.Example.Entity;
using Domain.Example.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Domain.PageExtend;

namespace Domain.Example.Reader
{
    public interface IExampleQueries
    {
        ExampleResult Get(Guid Id);

        Task<PageResult<ExampleResult>> GetListAsyncTest(PageCommand Cmd);
    }
}
