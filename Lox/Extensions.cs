using System.Collections.Generic;

namespace Lox
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
    }
}
