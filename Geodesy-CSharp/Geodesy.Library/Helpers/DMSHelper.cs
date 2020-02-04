/*
 * This is a C# implementation of the following:
 * 
 * https://www.movable-type.co.uk/scripts/geodesy/docs/dms.js.html
 * 
 */

using System;

namespace Geodesy.Library.Helpers
{
    public static class DMSHelper
    {
        /// <summary>
        /// Method that will restiuct the degrees between -180 and +180 degrees.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double Wrap180(double degrees)
        {
            if (-180 < degrees && degrees <= 180) return degrees; 
            return (degrees + 540) % 360 - 180; // sawtooth wave p:180, a:±180
        }

        /// <summary>
        /// Method that will restrict the degrees to the bounds of -90 to + 90.
        /// </summary>
        /// <param name="degrees">The degrees we want to check are in bounds</param>
        /// <returns>The wrapped values in degrees</returns>
        public static double Wrap90(double degrees)
        {
            if (-90 <= degrees && degrees <= 90)
            {
                return degrees;
            }
            return Math.Abs((degrees % 360 + 270) % 360 - 180) - 90;
        }

    }
}
