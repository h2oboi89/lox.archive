using System.Collections.Generic;
using System.Linq;

namespace GenerateAst
{
    static class Extensions
    {
        /// <summary>
        ///     Deconstructs an IEnumerable into a first and rest.
        /// </summary>
        /// <typeparam name="T">Sequence item type.</typeparam>
        /// <param name="sequence">Sequence to desconstruct</param>
        /// <param name="first">First item in the sequence. May be default value for <typeparamref name="T"/></param>
        /// <param name="rest">The rest of the sequence. May be empty.</param>
        public static void Deconstruct<T>(this IEnumerable<T> sequence, out T first, out IEnumerable<T> rest)
        {
            first = sequence.FirstOrDefault();
            rest = sequence.Skip(1);
        }

        /// <summary>
        ///     Deconstructs an IEnumerable into a first, second, and rest.
        /// </summary>
        /// <typeparam name="T">Sequence item type.</typeparam>
        /// <param name="sequence">Sequence to desconstruct</param>
        /// <param name="first">First item in the sequence. May be default value for <typeparamref name="T"/>.</param>
        /// <param name="second">Second item in the sequence. May be default value for <typeparamref name="T"/>.</param>
        /// <param name="rest">The rest of the sequence. May be empty.</param>
        public static void Deconstruct<T>(this IEnumerable<T> sequence, out T first, out T second, out IEnumerable<T> rest) => (first, (second, rest)) = sequence;

        /// <summary>
        ///     Returns a copy of this string with the first letter capitalized
        /// </summary>
        /// <param name="str">This string.</param>
        /// <returns>Copy of string with first letter capitalized.</returns>
        public static string ToUppercaseFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return char.ToUpper(str[0]) + str.Substring(1);
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

        /// <summary>
        ///     Combines string split and trim methods
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="split">Character to split on.</param>
        /// <returns>Trimmmed collection of strings resulting from split.</returns>
        public static IEnumerable<string> SplitTrim(this string str, char split)
        {
            return str.Split(split).Select(s => s.Trim());
        }
    }
}
