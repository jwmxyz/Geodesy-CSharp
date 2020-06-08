using Geodesy.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

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
        public IActionResult LatLonToOsGridRef(string lat, string lon)
        {
            try
            {
                var latitude = double.Parse(lat);
                var longitude = double.Parse(lon);
                return Ok(new LatLon_OsGridRef(latitude, longitude).ToOSGridRef());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert lat/lon to OS Grid Reference.");
            }
        }

        /// <summary>
        /// Will convert a latitide longitude into a UTM
        /// </summary>
        /// <param name="lat">The latitude of the position</param>
        /// <param name="lon">The longitude of the position</param>
        /// <returns>The UMT object or an error string</returns>
        [HttpGet("lat/{lat}/lon/{lon}/utm")]
        [ProducesResponseType(typeof(Utm), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult LatLonToUtm(string lat, string lon)
        {
            try
            {
                var latitude = double.Parse(lat);
                var longitude = double.Parse(lon);
                return Ok(new LatLon_Utm(latitude, longitude).ToUtm());
            } catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert lat/lon to UTM");
            }
        }

        /// <summary>
        /// Will convert a latitide longitude into a MGRS
        /// </summary>
        /// <param name="lat">The latitude of the position</param>
        /// <param name="lon">The longitude of the position</param>
        /// <returns>The MGRS object or an error string</returns>
        [HttpGet("lat/{lat}/lon/{lon}/mgrs")]
        [ProducesResponseType(typeof(Mgrs), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult LatLonToMgrs(string lat, string lon)
        {
            try
            {
                var latitude = double.Parse(lat);
                var longitude = double.Parse(lon);
                return Ok(new Utm_Mgrs(new LatLon_Utm(latitude, longitude).ToUtm()).ToMgrs());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert lat/lon yo MGRS");
            }
        }
    }
}