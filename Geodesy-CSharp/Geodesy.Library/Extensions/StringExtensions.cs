using System;
using System.Linq;

namespace Geodesy.Library.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Method that will remove any whitespace from a string
        /// </summary>
        /// <param name="currString">The string we want to remove any whitespace from</param>
        /// <returns>The string with any white space removed.</returns>
        public static string RemoveWhiteSpace(this string currString)
        {
            return new string(currString.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
