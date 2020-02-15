using Geodesy.Library;
using Xunit;

namespace Geodesy.Tests
{
    public class CartesianTests
    {
        [Fact]
        public void CartesianTestOne()
        {
            var latLong = new LatLonEllipsoidal(80, 80);
            var cartesian = latLong.ToCartesian();
            Assert.True(cartesian.X == 192434.54506198384);
            Assert.True(cartesian.Y == 1091350.5368764333);
            Assert.True(cartesian.Z == 6242764.1697421819);
        }

    }
}
