using Geodesy.Library;
using Xunit;

namespace Geodesy.Tests
{
    public class OsGridRefTests
    {
        [Fact]
        public void OsGridRefToStringIsCorrect()
        {
            var gridRef = new OsGridRef(651409, 313177);
            var stringResult = gridRef.ToString();
            Assert.Equal("TG 51409 13177", stringResult);

            gridRef = new OsGridRef(651409, 313177);
            stringResult = gridRef.ToString();
            Assert.Equal("TG 51409 13177", stringResult);
        }
    }
}
