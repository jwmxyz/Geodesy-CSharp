using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geodesy.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Geodesy_CSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LatLonController : ControllerBase
    {
        private readonly ILogger<LatLonController> _logger;

        public LatLonController(ILogger<LatLonController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Will convert a latitide longitude into a OSGridReference
        /// </summary>
        /// <param name="lat">The latitude of the position</param>
        /// <param name="lon">The longitude of the position</param>
        /// <returns>The latitude/longitude points or an error string</returns>
        [HttpGet("lat/{lat}/lon/{lon}/osGridRef")]
        [ProducesResponseType(typeof(OsGridRef), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult MgrsToLatLon(string lat, string lon)
        {
            try
            {
                var latitude = double.Parse(lat);
                var longitude = double.Parse(lon);
                return Ok(new LatLon_OsGridRef(latitude, longitude).ToOSGridRef().ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}