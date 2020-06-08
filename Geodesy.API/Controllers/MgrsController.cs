using Geodesy.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Geodesy_CSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MgrsController : ControllerBase
    {
        private readonly ILogger<MgrsController> _logger;

        public MgrsController(ILogger<MgrsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Will convert a given MGRS to a latitude / longitude coordinate
        /// </summary>
        /// <param name="mgrsReference">The string Mgrs reference</param>
        /// <returns>The latitude/longitude points or an error string</returns>
        [HttpGet("{mgrsReference}/latlon")]
        [ProducesResponseType(typeof(Mgrs), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult MgrsToLatLon(string mgrsReference)
        {
            try
            {
                return Ok(new Mgrs(mgrsReference).ToUtm().ToLatLon());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to parse MGRS to latlon");
            }
        }

        /// <summary>
        /// Will convert a given MGRS to a UTM coordinate
        /// </summary>
        /// <param name="mgrsReference">The string Mgrs reference</param>
        /// <returns>The UTM object or an error string</returns>
        [HttpGet("{mgrsReference}/utm")]
        [ProducesResponseType(typeof(Utm), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult MgrsToUtm(string mgrsReference)
        {
            try
            {
                return Ok(new Mgrs(mgrsReference).ToUtm());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Unable to parse MGRS to UTM");
            }
        }
    }
}
