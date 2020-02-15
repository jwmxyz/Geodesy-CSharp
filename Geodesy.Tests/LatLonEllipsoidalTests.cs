using Geodesy.Library;
using Geodesy.Library.Classes;
using Xunit;

namespace Geodesy.Tests
{
    public class LatLonEllipsoidalTests
    {
        [Fact]
        public void ToLatLon()
        {
            var c = new Cartesian(4027893.924, 307041.993, 4919474.294);
            var p = c.ToLatLon();
            Assert.True(p.Latitude == 50.798323358583062 && p.Longitude == 4.3591648034950277 && p.Height == 235.54027061536908);
        }
    }
}
