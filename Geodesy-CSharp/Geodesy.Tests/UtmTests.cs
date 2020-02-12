using Geodesy.Library;
using Geodesy.Library.Exceptions;
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
            Assert.Throws<ReferenceParsingException>(() => new Utm("random String"));
        }

        [Fact]
        public void ErrorIfZoneIsNonNumeric()
        {
            Assert.Throws<ReferenceParsingException>(() => new Utm("31 33N 448251 5411932"));
        }

        [Fact]
        public void ErrorIfZoneIsNotNorthOrSouth()
        {
            Assert.Throws<ReferenceParsingException>(() => new Utm("31 F 448251 5411932"));
        }

        [Fact]
        public void ErrorIfEastingIsNotANumber()
        {
            Assert.Throws<ReferenceParsingException>(() => new Utm("31 N sdfsdf 5411932"));
        }

        [Fact]
        public void ErrorIfNorthingIsNotANumber()
        {
            Assert.Throws<ReferenceParsingException>(() => new Utm("31 N 448251 sdfsdf"));
        }

        [Fact]
        public void ErrorIfZoneAboveSixty()
        {
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm("99 N 448251 5411932"));
        }

        [Fact]
        public void ErrorIfEastingIsAboveMax()
        {
            var testValue = 10000e3 + 1;
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm($"99 N {testValue.ToString()} 5411932"));
        }

        [Fact]
        public void ErrorIfNorthingIsAboveMaxNorth()
        {
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm($"99 N 448251 9328095"));
        }

        [Fact]
        public void ErrorIfNorthingIsBelowMinNorth()
        {
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm($"99 N 448251 -88"));
        }

        [Fact]
        public void ErrorIfNorthingIsAboveMaxSouth()
        {
            var testValue = 10000e3 + 1;
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm($"99 S 448251 {testValue.ToString()}"));
        }

        [Fact]
        public void ErrorIfNorthingIsBelowMinSouth()
        {
            Assert.Throws<InvalidReferencePropertyException>(() => new Utm($"99 S 448251 1118413"));
        }
    }
}
