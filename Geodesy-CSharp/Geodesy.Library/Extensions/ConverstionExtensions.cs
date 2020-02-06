using System;
using System.Collections.Generic;
using System.Text;

namespace Geodesy.Library.Extensions
{
    public static class ConverstionExtensions
    {
        public static double ToRadians(this double val)
        {
            return Math.PI / 180 * val;
        }

        public static double ToRadians(this int val)
        {
            return Math.PI / 180 * val;
        }

        public static double ToDegrees(this double val)
        {
            return val * 180 / Math.PI;
        }
    }
}
