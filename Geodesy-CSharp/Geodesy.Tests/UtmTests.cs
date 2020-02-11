using Geodesy.Library;
using System;
using Xunit;

namespace Geodesy.Tests
{
    public class UtmTests
    {
        [Fact]
        public void UtmToLatLon()
        {
            var gridRef = new Utm(31, 'N', 448251.795, 5411932.678);
            var latLon = gridRef.ToLatLon();
            Assert.True(latLon.Longitude == 58.402896764004851 && latLon.Latitude == 33.066837013130389);
        }

        [Fact]
        public void ErrorIfInvalidFormat()
        {
            Assert.Throws<Exception>(() => new Utm("random String"));
        }
    }
}
