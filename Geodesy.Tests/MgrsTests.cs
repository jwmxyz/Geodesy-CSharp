using Geodesy.Library;
using Geodesy.Library.Enums;
using Geodesy.Library.Exceptions;
using Xunit;

namespace Geodesy.Tests
{
    public class MgrsTests
    {
        [Fact]
        public void ValidMgrsConversionToUtm()
        {
            var mgrs = new Mgrs(31, 'U', 'D', 'Q', 48251, 11932);
            var result = mgrs.ToUtm();
            Assert.True(result.Northing == 5411932 && result.Easting == 448251 && result.Hemisphere == 'N' && result.Zone == 31);
        }

        [Fact]
        public void ValidMgrsFromString()
        {
            var mgrs = new Mgrs("31U DQ 48251 11932");
            Assert.True(mgrs.Band == 'U' && mgrs.Zone == 31 && mgrs.E100k == 'D' && mgrs.N100k == 'Q' && mgrs.Easting == 48251 && mgrs.Northing == 11932);
        }

        [Fact]
        public void ErrorIsThrownWithIncorrectZone()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("99U DQ 48251 11932"));
        }

        [Fact]
        public void ErrorIsThrownWithIncorrectBand()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("31I DQ 48251 11932"));
        }

        [Fact]
        public void ErrorIsThrownWithE100kIncorrect()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("31U IQ 48251 11932"));
        }

        [Fact]
        public void ErrorIsThrownWithN100kIncorrect()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("31U DI 48251 11932"));
        }

        [Fact]
        public void ErrorIsThrownWithEastingIncorrect()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("31U DI 99999 11932"));
        }

        [Fact]
        public void ErrorIsThrownWithNorthingIncorrect()
        {
            Assert.Throws<InvalidReferencePropertyException<MgrsEnum>>(() => new Mgrs("31U DI 48251 99999"));
        }

        [Fact]
        public void ErrorIsThrownWithIncorrectformat()
        {
            Assert.Throws<ReferenceParsingException>(() => new Mgrs("31U"));
        }
    }
}
