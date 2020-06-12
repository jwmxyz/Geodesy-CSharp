using Geodesy.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Geodesy_CSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OsGridrefController : ControllerBase
    {
        private readonly ILogger<OsGridrefController> _logger;

        public OsGridrefController(ILogger<OsGridrefController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Will convert a given OSGridReference to a latitude / longitude coordinate
        /// </summary>
        /// <param name="OsGridReference">The string OsGridRefence reference</param>
        /// <returns>The latitude/longitude points or an error string</returns>
        [HttpGet("{OsGridReference}/latlon")]
        [ProducesResponseType(typeof(LatLon_OsGridRef), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult OsGridrefToLatLon(string OsGridReference)
        {
            try
            {
                return Ok(new OsGridRef(OsGridReference).ToLatLon());
            } catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert from OS Grid Reference to LatLon.");
            }
        }

        /// <summary>
        /// Will convert a given OSGridReference to a LatLon UTM coordinate.
        /// </summary>
        /// <param name="OsGridReference">The string OsGridRefence reference</param>
        /// <returns>The UTM object or an error string</returns>
        [HttpGet("{OsGridReference}/utm")]
        [ProducesResponseType(typeof(Utm), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult OsGridRefToUtm(string OsGridReference)
        {
            try
            {
                var osGridRefLatLon = new OsGridRef(OsGridReference).ToLatLon();
                return Ok(new LatLon_Utm(osGridRefLatLon.Latitude, osGridRefLatLon.Longitude).ToUtm());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to convert from OS Grid Reference to UTM.");
            }
        }
    }
}