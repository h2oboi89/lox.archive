using System.Linq;

namespace CodeGeneration.Utilities
{
    static class Extensions
    {
        /// <summary>
        ///     Returns a copy of this string repeated count times.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="count">Number of times to repeat this string.</param>
        /// <returns>
        ///     Empty string for if this string is null or count is less than or equal to 0; 
        ///     otherwise this string repeated count times.
        /// </returns>
        public static string Repeat(this string str, int count)
        {
            if (count <= 0 || string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return string.Concat(Enumerable.Repeat(str, count));
        }
    }
}
