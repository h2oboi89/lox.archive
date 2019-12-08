using System.Collections.Generic;
using System.Linq;

namespace GenerateParseRules
{
    static class Extensions
    {
        public struct EnumeratedInstance<T>
        {
            public int Index;
            public T Value;
        }

        /// <summary>
        ///     Enumerates over IEnumerable
        /// </summary>
        /// <typeparam name="T">IEnumerable type.</typeparam>
        /// <param name="collection">Collection to enumerate over</param>
        /// <returns>Collection of structs with Index and Value of current item during iteration.</returns>
        public static IEnumerable<EnumeratedInstance<T>> Enumerate<T>(this IEnumerable<T> collection)
        {
            var i = 0;
            foreach (var item in collection)
            {
                yield return new EnumeratedInstance<T> { Index = i++, Value = item };
            }
        }

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
