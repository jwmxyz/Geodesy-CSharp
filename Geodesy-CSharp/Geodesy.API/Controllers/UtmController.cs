using Geodesy.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Geodesy_CSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtmController : ControllerBase
    {
        private readonly ILogger<UtmController> _logger;

        public UtmController(ILogger<UtmController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Will convert a given UTM string into the corresponding Latitude and Longitude Coordinates.
        /// </summary>
        /// <param name="utmReference">The string UTM reference</param>
        /// <returns>The latitude/longitude points or an error string</returns>
        [HttpGet("latlon/{utmReference}")]
        [ProducesResponseType(typeof(LatLon_Utm), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult UtmToLatLon(string utmReference)
        {
            try
            {
                return Ok(new Utm(utmReference).ToLatLon());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
