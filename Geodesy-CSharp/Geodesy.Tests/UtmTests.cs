using Geodesy.Library;
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
            Assert.True(false);
        }
    }
}
