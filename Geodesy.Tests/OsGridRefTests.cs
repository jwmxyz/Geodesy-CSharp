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
            var stringResult = gridRef.ToString(8);
            Assert.Equal("TG 5140 1317", stringResult);

            gridRef = new OsGridRef(651409, 313177);
            stringResult = gridRef.ToString(0);
            Assert.Equal("651409,313177", stringResult);
        }
    }
}
