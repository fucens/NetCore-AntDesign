using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Example.Commands;
using Domain.Example.Reader;
using Domain.Example.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [EnableCors("any")]
    [Route("[controller]/[action]")]
    public class ExampleController : Controller
    {
        private readonly IExampleRepository _exampleRepository;
        private readonly IExampleQueries _exampleQueries;
        public ExampleController(IExampleRepository exampleRepository,
           IExampleQueries exampleQueries)
        {
            _exampleRepository = exampleRepository;
            _exampleQueries = exampleQueries;
        }

        //public IActionResult Test()
        //{
        //    return Ok(new { aaaa = "aaaaa" });
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCommand Cmd)
        {
            await _exampleRepository.CreateAsync(Cmd);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> GetList([FromBody]PageCommand Cmd)
        {
            var result = await _exampleQueries.GetListAsyncTest(Cmd);
            return Ok(result);
        }


    }
}