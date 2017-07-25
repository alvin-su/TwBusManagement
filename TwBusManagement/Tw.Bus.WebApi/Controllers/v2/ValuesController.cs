using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tw.Bus.WebApi.Controllers.v2
{
    [ApiVersion("2.0")]
   // [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet, MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value2" };
        }

    }
}
