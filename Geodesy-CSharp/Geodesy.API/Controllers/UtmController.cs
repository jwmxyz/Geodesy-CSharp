using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geodesy_CSharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtmController : ControllerBase
    {
        private readonly ILogger<UtmController> _logger;

        public UtmController(ILogger<UtmController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult UtmToLatLon()
        {
            return Ok();
        }
    }
}
