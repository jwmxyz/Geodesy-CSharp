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
        /// Will convert a given MGRS to a latitude / longitude coordinate
        /// </summary>
        /// <param name="OsGridReference">The string OsGridRefence reference</param>
        /// <returns>The latitude/longitude points or an error string</returns>
        [HttpGet("latlon/{OsGridReference}")]
        [ProducesResponseType(typeof(LatLon_OsGridRef), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult OsGridrefToLatLon(string OsGridReference)
        {
            try
            {
                return Ok(new OsGridRef(OsGridReference).ToLatLon());
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}