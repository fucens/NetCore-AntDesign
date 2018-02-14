using Domain.Example.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Example.Writer
{
    public interface IExampleRepository
    {
        void Create(CreateCommand Cmd);
        Task CreateAsync(CreateCommand Cmd);
    }
}
