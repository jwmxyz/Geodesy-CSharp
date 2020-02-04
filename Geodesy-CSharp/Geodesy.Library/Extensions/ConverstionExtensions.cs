using System;
using System.Collections.Generic;
using System.Text;

namespace Geodesy.Library.Extensions
{
    public static class ConverstionExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
