using Geodesy.Library;
using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}
