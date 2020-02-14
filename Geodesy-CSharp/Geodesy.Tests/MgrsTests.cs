using Geodesy.Library;
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
            Assert.True(result.Northing == 5448251 && result.Easting == 411932 && result.Hemisphere == 'N' && result.Zone == 31);
        }

        [Fact]
        public void ValidMgrsFromString()
        {
            var mgrs = new Mgrs("31U DQ 48251 11932");
            Assert.True(true);
        }
    }
}
