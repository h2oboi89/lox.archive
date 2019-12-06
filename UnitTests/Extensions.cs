using System.Collections.Generic;

namespace UnitTests
{
    static class Extensions
    {
        internal struct EnumeratedInstance<T>
        {
            public int Index;
            public T Value;
        }

        internal static IEnumerable<EnumeratedInstance<T>> Enumerate<T>(this IEnumerable<T> collection)
        {
            var i = 0;
            foreach (var item in collection)
            {
                yield return new EnumeratedInstance<T> { Index = i++, Value = item };
            }
        }
    }
}
