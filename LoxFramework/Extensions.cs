using System.Collections.Generic;

namespace LoxFramework
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
        ///     Similar to <see cref="string.Substring(int, int)"/>.
        /// </summary>
        /// <param name="str">String to extract substring from.</param>
        /// <param name="startIndex">The inclusive zero-based starting character position of a substring in this instance.</param>
        /// <param name="endIndex">The exclusive zero-based ending character position of a substring in this instance.</param>
        /// <returns>
        ///     A string that is equivalent to the substring that begins at
        ///     startIndex in this instance and ends before endIndex, or 
        ///     System.String.Empty if start is equal to the length of this 
        ///     instance or endIndex.
        /// </returns>
        public static string Extract(this string str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
}
