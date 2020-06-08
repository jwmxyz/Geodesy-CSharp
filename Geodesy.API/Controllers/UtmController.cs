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
        [HttpGet("{utmReference}/latlon")]
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
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert from UTM to LatLon.");
            }
        }

        /// <summary>
        /// Will convert a given UTM string into a MGRS object.
        /// </summary>
        /// <param name="utmReference">The string UTM reference</param>
        /// <returns>The MGRS point or an error string</returns>
        [HttpGet("{utmReference}/mgrs")]
        [ProducesResponseType(typeof(Mgrs), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult UtmToMgrs(string utmReference)
        {
            try
            {
                return Ok(new Utm_Mgrs(utmReference).ToMgrs());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert from UTM to MGRS.");
            }
        }
    }
}