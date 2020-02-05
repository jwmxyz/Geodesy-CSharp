using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Geodesy_CSharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MgrsController : ControllerBase
    {
        private readonly ILogger<MgrsController> _logger;

        public MgrsController(ILogger<MgrsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult Post()
        {
            return Ok();
        }
    }
}
