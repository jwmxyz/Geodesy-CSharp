using System;

namespace Geodesy.Library.Extensions
{
    public static class ConverstionExtensions
    {
        /// <summary>
        /// Converts a int value into Radians
        /// </summary>
        /// <param name="val">The value that we want to convert into val</param>
        /// <returns>The radians value that has been converted</returns>
        public static double ToRadians(this double val)
        {
            return Math.PI / 180 * val;
        }

        /// <summary>
        /// Converts a int value into Radians
        /// </summary>
        /// <param name="val">The value that we want to convert into val</param>
        /// <returns>The radians value that has been converted</returns>
        public static double ToRadians(this int val)
        {
            return Math.PI / 180 * val;
        }

        /// <summary>
        /// Converts a value into degrees
        /// </summary>
        /// <param name="val">The value that we want to convert into degrees</param>
        /// <returns>The value in degrees</returns>
        public static double ToDegrees(this double val)
        {
            return val * 180 / Math.PI;
        }
    }
}
